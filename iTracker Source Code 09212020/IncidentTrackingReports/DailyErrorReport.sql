USE AUTO_IncidentTrack
GO

DECLARE @errorType VARCHAR(20) = 'Controls Code Error';
DECLARE @areaId INT = 100;
DECLARE @type VARCHAR(3) = 'PCS';
DECLARE @style NVARCHAR(2000);
DECLARE @msg NVARCHAR(200);
DECLARE @html NVARCHAR(MAX);

SET NOCOUNT ON;

--set up email style information
SET @style = N'<html><head><style type="text/css">
        body { font-family: Segoe UI; color: #A0A0A5; padding: 10px; }
        table { border: none; border-collapse: collapse; }
        td, th { border: none; font-size: 10pt; padding: 4px 8px; text-align: left; }
        th { color: #808085; font-weight: bold; border-bottom: 2px solid #808085; }
        td { border-bottom: 1px solid #F0F0F5; }
        caption { color: #2A4B7C; font-size: 14pt; text-align: left; font-weight: bold; padding-bottom: 5px; }
        a { color: #2A4B7C; }
        .text-right { text-align: right; }
    </style></head>';

SET @msg = '<body><p>One or more user/control errors were reported in your area within the last 24 hours. Please click [Details] to
        see a full description of each event.</p>';

--construct HTML table of the data
SET @html = @style + @msg + N'<table><tr><th>Date/Time</th><th>Engineer Name</th><th>Control System</th><th>DT (h)</th><th>Engn Time (h)</th><th>Details</th></tr>' +
	CAST(( SELECT td = CONVERT(VARCHAR(16), a.ActivityPerformedDateTime, 121), '',
			td = a.EngineerName, '',
			td = a.ControlSystem, '',
			tdr = FORMAT(a.EstimatedDownTimeHours, 'F1'), '',
			tdr = FORMAT(a.EngineeringTimeHours, 'F1'), '',
			tda = URL
		FROM (
			SELECT 
					i.IncidentId
				, CAST(i.LocalActivityPerformedDateTime AS smalldatetime) [ActivityPerformedDateTime]
				, i.EngineerName
				, s.[Name] [ControlSystem]
				, i.EstimatedDownTimeHours
				, i.EngineeringTimeHours
				, i.IncidentDescription
				, i.ActionSummary
				, 'http://pwkanweb001/AutoIncidentTracking/Incidents/Details/' + CAST(i.IncidentId AS VARCHAR) [URL]
			FROM [AUTO_IncidentTrack].dbo.Incidents i
			INNER JOIN [AUTO_IncidentTrack].dbo.ControlSystems s ON i.ControlSystemId = s.ControlSystemId
			INNER JOIN [AUTO_IncidentTrack].dbo.Classifications c ON i.ClassificationId = c.ClassificationId
			WHERE i.LocalActivityPerformedDateTime >= DATEADD(DAY, -1, GETDATE())
				AND c.[Name] IN ('User Error/Controls Code Error')
				--AND i.ManufacturingAreaId = @areaId
				AND @type = 'PCS'
			UNION
			SELECT 
				  i.IncidentId
				, CAST(i.LocalActivityPerformedDateTime AS smalldatetime) [ActivityPerformedDateTime]
				, i.EngineerName
				, 'N/A' [ControlSystem]
				, i.EstimatedDownTimeHours
				, i.EngineeringTimeHours
				, i.IncidentDescription
				, i.ActionSummary
				, 'http://pwkanweb001/MesIncidentTracking/Incidents/Details/' + CAST(i.IncidentId AS VARCHAR) [URL]
			FROM [MES_IncidentTrack].dbo.Incidents i
			INNER JOIN [MES_IncidentTrack].dbo.Classifications c ON i.ClassificationId = c.ClassificationId
			WHERE i.LocalActivityPerformedDateTime >= DATEADD(DAY, -1, GETDATE())
				AND c.[Name] IN ('User Error/Controls Code Error')
				--AND i.ManufacturingAreaId = @areaId
				AND @type = 'MES'
		) a
		ORDER BY 2
		FOR XML RAW('tr'), ELEMENTS) AS NVARCHAR(MAX)) + N'</table></body></html>';

SET @html = REPLACE(REPLACE(@html, '<tda>', '<td><a href="'), '</tda>','">Details</a></td>')
SET @html = REPLACE(REPLACE(@html, '<tdr>', '<td class="text-right">'), '</tdr>', '</td>')

SELECT @html
EXEC msdb.dbo.sp_send_dbmail @recipients = 'brad.robbins@cslbehring.com', @subject = 'Testing email', @body = @html, @body_format = 'HTML'