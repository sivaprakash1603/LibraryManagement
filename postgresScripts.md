-- Function: calculate_member_fine
CREATE OR REPLACE FUNCTION calculate_member_fine(p_memberid integer)
RETURNS numeric AS $$
DECLARE
  v_total numeric;
BEGIN
  SELECT COALESCE(SUM(f.fineamount), 0)::numeric INTO v_total
  FROM fines f
  JOIN borrowings b ON f.borrowingid = b.id
  WHERE b.memberid = p_memberid
    AND f.ispaid = false;
  RETURN v_total;
END;
$$ LANGUAGE plpgsql STABLE;

-- Function: get_available_books_by_category
CREATE OR REPLACE FUNCTION get_available_books_by_category(p_categoryid integer)
RETURNS TABLE(isbn varchar, title varchar, authorname varchar, available_count integer) AS $$
BEGIN
  RETURN QUERY
  SELECT b.isbn, b.title, b.authorname, COUNT(bc.barcodeno)::integer AS available_count
  FROM books b
  JOIN bookcopies bc ON bc.isbn = b.isbn
  WHERE b.categoryid = p_categoryid
    AND bc.status = 'Available'
  GROUP BY b.isbn, b.title, b.authorname;
END;
$$ LANGUAGE plpgsql STABLE;

-- Function: get_member_borrowing_summary
CREATE OR REPLACE FUNCTION get_member_borrowing_summary(p_memberid integer)
RETURNS TABLE(active_borrows integer, returned_borrows integer, unpaid_fine numeric) AS $$
BEGIN
  RETURN QUERY
  SELECT
    COUNT(*) FILTER (WHERE b.borrowstatus = 'Borrowed')::integer AS active_borrows,
    COUNT(*) FILTER (WHERE b.borrowstatus = 'Returned')::integer AS returned_borrows,
    COALESCE((SELECT SUM(f.fineamount) FROM fines f JOIN borrowings bb ON f.borrowingid = bb.id WHERE bb.memberid = p_memberid AND f.ispaid = false), 0)::numeric AS unpaid_fine
  FROM borrowings b
  WHERE b.memberid = p_memberid;
END;
$$ LANGUAGE plpgsql STABLE;

-- Function: create_borrowing
CREATE OR REPLACE FUNCTION create_borrowing(p_memberid integer, p_barcodeno varchar)
RETURNS integer AS $$
DECLARE
  v_mt_id integer;
  v_accountstatus varchar;
  v_max_borrowings integer;
  v_max_borrowdays integer;
  v_active_borrows integer;
  v_unpaid_fine numeric := 0;
  v_isbn varchar;
  v_already_borrowed boolean;
  v_status varchar;
  v_new_borrowing_id integer;
BEGIN
  SELECT accountstatus, membershiptypeid INTO v_accountstatus, v_mt_id
  FROM members WHERE id = p_memberid;
  IF NOT FOUND THEN
    RAISE EXCEPTION 'Member % not found', p_memberid;
  END IF;
  IF v_accountstatus IS NULL OR v_accountstatus <> 'Active' THEN
    RAISE EXCEPTION 'Member % is not active', p_memberid;
  END IF;

  SELECT maximumborrowings, maximumborrowdays INTO v_max_borrowings, v_max_borrowdays
  FROM membershiptypes WHERE id = v_mt_id;
  IF NOT FOUND THEN
    RAISE EXCEPTION 'Membership type % not found for member %', v_mt_id, p_memberid;
  END IF;

  SELECT COALESCE(SUM(f.fineamount),0) INTO v_unpaid_fine
  FROM fines f JOIN borrowings b ON f.borrowingid = b.id
  WHERE b.memberid = p_memberid AND f.ispaid = false;
  IF v_unpaid_fine > 500 THEN
    RAISE EXCEPTION 'Member % has unpaid fines (%.2f) above limit', p_memberid, v_unpaid_fine;
  END IF;

  SELECT COUNT(*) INTO v_active_borrows
  FROM borrowings WHERE memberid = p_memberid AND borrowstatus = 'Borrowed';
  IF v_active_borrows >= v_max_borrowings THEN
    RAISE EXCEPTION 'Member % has reached maximum active borrowings (%).', p_memberid, v_max_borrowings;
  END IF;

  SELECT isbn INTO v_isbn FROM bookcopies WHERE barcodeno = p_barcodeno;
  IF NOT FOUND OR v_isbn IS NULL THEN
    RAISE EXCEPTION 'Book copy % not found', p_barcodeno;
  END IF;

  SELECT EXISTS(
    SELECT 1 FROM borrowings b
    JOIN bookcopies bc ON b.barcodeno = bc.barcodeno
    WHERE b.memberid = p_memberid AND b.borrowstatus = 'Borrowed' AND bc.isbn = v_isbn
  ) INTO v_already_borrowed;
  IF v_already_borrowed THEN
    RAISE EXCEPTION 'Member % already has an active borrowing for ISBN %', p_memberid, v_isbn;
  END IF;

  SELECT status INTO v_status FROM bookcopies WHERE barcodeno = p_barcodeno;
  IF v_status IS DISTINCT FROM 'Available' THEN
    RAISE EXCEPTION 'Book copy % is not available (status: %)', p_barcodeno, v_status;
  END IF;

  INSERT INTO borrowings(memberid, barcodeno, borrowdate, duedate, borrowstatus)
  VALUES (p_memberid, p_barcodeno, CURRENT_DATE, CURRENT_DATE + v_max_borrowdays, 'Borrowed')
  RETURNING id INTO v_new_borrowing_id;

  UPDATE bookcopies SET status = 'Borrowed' WHERE barcodeno = p_barcodeno;

  RETURN v_new_borrowing_id;
END;
$$ LANGUAGE plpgsql VOLATILE;

-- Procedure: return_book
CREATE OR REPLACE PROCEDURE return_book(p_barcodeno varchar, p_return_date date DEFAULT CURRENT_DATE)
LANGUAGE plpgsql
AS $$
DECLARE
    v_borrowing_id integer;
    v_due_date date;
    v_delay_days integer := 0;
    v_fine_amount numeric := 0;
BEGIN
    SELECT b.id, b.duedate INTO v_borrowing_id, v_due_date
    FROM borrowings b
    WHERE b.barcodeno = p_barcodeno
      AND b.borrowstatus = 'Borrowed'
    ORDER BY b.borrowdate DESC
    LIMIT 1;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'No active borrowing found for barcode %', p_barcodeno;
    END IF;

    IF p_return_date > v_due_date THEN
        v_delay_days := (p_return_date - v_due_date);
        v_fine_amount := v_delay_days * 10;
    END IF;

    UPDATE borrowings
    SET returndate = p_return_date,
        borrowstatus = 'Returned'
    WHERE id = v_borrowing_id;

    UPDATE bookcopies
    SET status = 'Available'
    WHERE barcodeno = p_barcodeno;

    IF v_fine_amount > 0 THEN
        UPDATE fines
        SET fineamount = v_fine_amount, ispaid = false, finetype = 'LateReturn'
        WHERE borrowingid = v_borrowing_id AND finetype = 'LateReturn';

        IF NOT FOUND THEN
            INSERT INTO fines(borrowingid, finetype, fineamount, ispaid)
            VALUES (v_borrowing_id, 'LateReturn', v_fine_amount, false);
        END IF;
    END IF;
END;
$$;