USE msdb;
GO


/************************************************************************************
 * JOB:				[Incident Tracker Daily Error Report] 
 * PURPOSE:			Get and send the daily error report
 * CREATED BY:		Brad Robbins (brobbins@e2i.net)
 * CREATED DATE:	2020-03-17
 *-----------------------------------------------------------------------------------
 * REVISED BY	DATE		DESCRIPTION
 * B.Robbins	2020-03-17	Initial version
 ************************************************************************************/
DECLARE @jobId BINARY(16);
DECLARE @now INT = CONVERT(INT, CONVERT(VARCHAR(8), GETUTCDATE(), 112));

--Add a named job
EXEC dbo.sp_add_job 
    @job_name = N'Incident Tracker Daily Error Report', 
	@description = N'Automatic daily error report to area engineers',
    @job_id = @jobId OUTPUT;

--Add a step (operation) to the job
EXEC sp_add_jobstep
   @job_id = @jobId,
   @step_name = N'Send Daily Error Report',
   @step_id = 1,
   @subsystem = N'TSQL',
   @command=N'EXEC AUTO_IncidentTrack.dbo.SendDailyErrorReport', 
   @database_name=N'AUTO_IncidentTrack', 
   @retry_attempts = 5,
   @retry_interval = 5;

-- Creates a schedule daily at 7AM and attach it to job
EXEC sp_add_jobschedule 
    @job_id=@jobId, 
    @name=N'Daily Check', 
    @enabled=1, 
    @freq_type=4, 
    @freq_interval=1, 
    @freq_subday_type=1, 
    @freq_subday_interval=0, 
    @freq_relative_interval=0, 
    @freq_recurrence_factor=0, 
    @active_start_date=@now, 
    @active_end_date=99991231, 
    @active_start_time=70000, 
    @active_end_time=235959;

--Add the job to the server
EXEC dbo.sp_add_jobserver @job_id=@jobId, @server_name = N'PWKANSQL001';
GO

/************************************************************************************
 * JOB:				[Incident Tracker Daily Incident Report] 
 * PURPOSE:			Get and send the daily incident report
 * CREATED BY:		Brad Robbins (brobbins@e2i.net)
 * CREATED DATE:	2020-03-18
 *-----------------------------------------------------------------------------------
 * REVISED BY	DATE		DESCRIPTION
 * B.Robbins	2020-03-18	Initial version
 ************************************************************************************/
DECLARE @jobId BINARY(16);
DECLARE @now INT = CONVERT(INT, CONVERT(VARCHAR(8), GETUTCDATE(), 112));

--Add a named job
EXEC dbo.sp_add_job 
    @job_name = N'Incident Tracker Daily Incident Report', 
	@description = N'Automatic daily incident report to area engineers',
    @job_id = @jobId OUTPUT;

--Add a step (operation) to the job
EXEC sp_add_jobstep
   @job_id = @jobId,
   @step_name = N'Send Daily Incident Report',
   @step_id = 1,
   @subsystem = N'TSQL',
   @command=N'EXEC AUTO_IncidentTrack.dbo.SendDailyIncidentReport', 
   @database_name=N'AUTO_IncidentTrack', 
   @retry_attempts = 5,
   @retry_interval = 5;

-- Creates a schedule daily at 7AM and attach it to job
EXEC sp_add_jobschedule 
    @job_id=@jobId, 
    @name=N'Daily Check', 
    @enabled=1, 
    @freq_type=4, 
    @freq_interval=1, 
    @freq_subday_type=1, 
    @freq_subday_interval=0, 
    @freq_relative_interval=0, 
    @freq_recurrence_factor=0, 
    @active_start_date=@now, 
    @active_end_date=99991231, 
    @active_start_time=70000, 
    @active_end_time=235959;

--Add the job to the server
EXEC dbo.sp_add_jobserver @job_id=@jobId, @server_name = N'PWKANSQL001';
GO