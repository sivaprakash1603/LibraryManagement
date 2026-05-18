# PostgreSQL Scripts

## Return Book Procedure

This procedure returns an active borrowing, marks the book copy as available, and generates a late fine when needed.

```sql
CREATE OR REPLACE PROCEDURE return_book(
	p_barcodeno VARCHAR,
	p_returndate DATE DEFAULT CURRENT_DATE
)
LANGUAGE plpgsql
AS $$
DECLARE
	v_borrowing_id INT;
	v_due_date DATE;
	v_late_days INT;
	v_fine_amount NUMERIC(10,2);
BEGIN
	SELECT b.id, b.duedate
	INTO v_borrowing_id, v_due_date
	FROM borrowings b
	WHERE b.barcodeno = p_barcodeno
	  AND b.borrowstatus = 'Borrowed'
	ORDER BY b.id DESC
	LIMIT 1;

	IF v_borrowing_id IS NULL THEN
		RAISE EXCEPTION 'No active borrowing found for barcode %', p_barcodeno;
	END IF;

	UPDATE borrowings
	SET returndate = p_returndate,
		borrowstatus = 'Returned'
	WHERE id = v_borrowing_id;

	UPDATE bookcopies
	SET status = 'Available'
	WHERE barcodeno = p_barcodeno;

	IF p_returndate > v_due_date THEN
		v_late_days := p_returndate - v_due_date;
		v_fine_amount := v_late_days * 10;

		INSERT INTO fines(borrowingid, finetype, fineamount, ispaid)
		VALUES (v_borrowing_id, 'LateReturn', v_fine_amount, FALSE);
	END IF;
END;
$$;
```

## Borrowing Function

```sql
CREATE OR REPLACE FUNCTION create_borrowing(p_member_id INT, p_barcodeno VARCHAR)
RETURNS INT
LANGUAGE plpgsql
AS $$
DECLARE
	v_max_days INT;
	v_new_id INT;
BEGIN
	SELECT mt.maximumborrowdays
	INTO v_max_days
	FROM members m
	JOIN membershiptypes mt ON mt.id = m.membershiptypeid
	WHERE m.id = p_member_id;

	IF v_max_days IS NULL THEN
		RAISE EXCEPTION 'Member % not found or membership type missing', p_member_id;
	END IF;

	INSERT INTO borrowings(memberid, barcodeno, borrowdate, duedate, borrowstatus)
	VALUES (
		p_member_id,
		p_barcodeno,
		CURRENT_DATE,
		CURRENT_DATE + v_max_days,
		'Borrowed'
	)
	RETURNING id INTO v_new_id;

	UPDATE bookcopies
	SET status = 'Borrowed'
	WHERE barcodeno = p_barcodeno;

	RETURN v_new_id;
END;
$$;
```

## Fine Calculation Function

```sql
CREATE OR REPLACE FUNCTION calculate_member_fine(p_member_id INT)
RETURNS NUMERIC(10,2)
LANGUAGE sql
AS $$
	SELECT COALESCE(SUM(f.fineamount), 0)::NUMERIC(10,2)
	FROM fines f
	JOIN borrowings b ON b.id = f.borrowingid
	WHERE b.memberid = p_member_id
	  AND f.ispaid = FALSE;
$$;
```

