DECLARE @bdy NVARCHAR(MAX);

SET NOCOUNT ON;

--set up email style information
DECLARE @style NVARCHAR(800) = N'<html><head><style type="text/css">
        body { font-family: Segoe UI; color: #A0A0A5; padding: 10px; }
        table { border: none; border-collapse: collapse; }
        td, th { border: none; font-size: 10pt; padding: 4px 8px; text-align: left; }
        th { color: #808085; font-weight: bold; border-bottom: 2px solid #808085; }
        td { border-bottom: 1px solid #F0F0F5; }
        caption { color: #2A4B7C; font-size: 14pt; text-align: left; font-weight: bold; padding-bottom: 5px; }
        a { color: #2A4B7C; }
        .text-right { text-align: right; }
    </style></head>';

SET @bdy = @style + N'<table><caption>Lastest Incidents</caption><tr><th>Type</th><th>Date/Time</th><th>Area</th><th>Control System</th><th>Classification</th><th>Engineer</th><th>Est. D/T (h)</th><th>Details</th></tr>' +
CAST((SELECT td = [Type], '',
			 td = [ActivityPerformedDateTime], '',
			 td = [Area], '',
			 td = [ControlSystem], '',
			 td = [RootCause], '',
			 td = [EngineerName], '',
			 tdr = FORMAT([EstimatedDownTime], 'F1'), '',
			 tda = [URL]
FROM (
	SELECT 'PCS' [Type]
		, CONVERT(VARCHAR(16), i.LocalActivityPerformedDateTime, 121) ActivityPerformedDateTime
		, m.[Name] Area
		, s.[Name] ControlSystem
		, c.[Name] RootCause
		, i.EngineerName
		, i.EstimatedDownTimeHours [EstimatedDowntime]
		, 'http://pwkanweb001/AutoIncidentTracking/Incidents/Details/' + CAST(i.IncidentId AS VARCHAR) [URL]
	FROM AUTO_IncidentTrack.dbo.Incidents i
	INNER JOIN [AUTO_IncidentTrack].dbo.ControlSystems s ON i.ControlSystemId = s.ControlSystemId
	INNER JOIN AUTO_IncidentTrack.dbo.ManufacturingAreas m ON i.ManufacturingAreaId = m.ManufacturingAreaId
	INNER JOIN AUTO_IncidentTrack.dbo.Classifications c ON i.ClassificationId = c.ClassificationId
	WHERE [LocalActivityPerformedDateTime] > DATEADD(DAY, -1, GETDATE())
	UNION
	SELECT 'MES' [Type]
		, CONVERT(VARCHAR(16), i.LocalActivityPerformedDateTime, 121) ActivityPerformedDateTime
		, m.[Name] Area
		, 'N/A' ControlSystem
		, c.[Name] RootCause
		, i.EngineerName
		, i.EstimatedDownTimeHours [EstimatedDowntime]
		, 'http://pwkanweb001/MesIncidentTracking/Incidents/Details/' + CAST(i.IncidentId AS VARCHAR) [URL]
	FROM MES_IncidentTrack.dbo.Incidents i
	INNER JOIN MES_IncidentTrack.dbo.ManufacturingAreas m ON i.ManufacturingAreaId = m.ManufacturingAreaId
	INNER JOIN MES_IncidentTrack.dbo.Classifications c ON i.ClassificationId = c.ClassificationId
	WHERE [LocalActivityPerformedDateTime] > DATEADD(DAY, -1, GETDATE())
) a
ORDER BY ActivityPerformedDateTime Desc, Area, EngineerName
FOR XML RAW('tr'), ELEMENTS) AS NVARCHAR(MAX)) + N'</table></body></html>';

SET @bdy = REPLACE(REPLACE(@bdy, '<tda>', '<td><a href="'), '</tda>','">Details</a></td>');
SET @bdy = REPLACE(REPLACE(@bdy, '<tdr>', '<td class="text-right">'), '</tdr>', '</td>');

SELECT @bdy;