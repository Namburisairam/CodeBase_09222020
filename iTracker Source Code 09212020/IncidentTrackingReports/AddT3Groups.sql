CREATE TABLE dbo.T3Groups (
    T3GroupId INT PRIMARY KEY,
    [Name] NVARCHAR(50) UNIQUE,
    SmeName NVARCHAR(50),
    SmeEmailAddres NVARCHAR(100),
    ReplyToEmailAddress NVARCHAR(100)
);
GO

INSERT INTO dbo.T3Groups VALUES
	(1, 'Critical Systems', 'Michael Schoonmaker', 'Michael.Schoonmaker@cslbehring.com', 'dennis.prom@cslbehring.com'),
	(2, 'Motive Power', 'Michael Schoonmaker', 'Michael.Schoonmaker@cslbehring.com', 'dennis.prom@cslbehring.com');
GO

ALTER TABLE dbo.ManufacturingAreas 
	ADD T3GroupId INT,
	CONSTRAINT [ManufacturingAreas_T3Groups_FK] 
		FOREIGN KEY([T3GroupId])
		REFERENCES dbo.[T3Groups] ([T3GroupId]);
GO


UPDATE dbo.ManufacturingAreas
SET T3GroupId = 1
WHERE [Name] IN (
    'Alcohol'
  , 'Caustic'
  , 'DI Water'
  , 'Pure Steam'
  , 'Tank Farm'
  , 'WFI and PW'
);

UPDATE dbo.ManufacturingAreas
SET T3GroupId = 2
WHERE [Name] IN (
    'Bio-Waste Grinder'
  , 'Glycol'
  , 'Plant Steam'
  , 'Process Water'
);

SELECT 
	t.T3GroupId,
	t.Name,
	m.ManufacturingAreaId,
	m.Name
FROM dbo.T3Groups t
LEFT JOIN dbo.ManufacturingAreas m ON t.T3GroupId = m.T3GroupId
