-- Manual schema cleanup: removes the "UgyfelTipus" column from the Ugyfelek
-- table. The Kft/Bt/Rt/... jogi forma classification was not used anywhere
-- in the business logic (no filtering, no template substitution), only
-- stored and displayed, so it has been removed from the code as well.
-- Run this against the live database after deploying the related code changes.

IF EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[Ugyfelek]') AND name = 'UgyfelTipus'
)
BEGIN
	ALTER TABLE [Ugyfelek]
		DROP COLUMN [UgyfelTipus];
END
GO
