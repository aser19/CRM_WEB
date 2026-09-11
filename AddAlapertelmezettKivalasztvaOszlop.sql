-- Manual schema change: adds the "AlapertelmezettKivalasztva" column to the
-- MeresTipusJogszabalyok join table, supporting the 3-state jogszabály/szabvány
-- hozzárendelés (assigned+preselected / assigned+not preselected / not assigned).
-- Run this against the live database after deploying the related code changes.

IF NOT EXISTS (
	SELECT 1 FROM sys.columns
	WHERE object_id = OBJECT_ID(N'[MeresTipusJogszabalyok]') AND name = 'AlapertelmezettKivalasztva'
)
BEGIN
	ALTER TABLE [MeresTipusJogszabalyok]
		ADD [AlapertelmezettKivalasztva] bit NOT NULL DEFAULT 1;
END
GO
