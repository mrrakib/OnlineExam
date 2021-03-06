/*

EXEC GetDashboardInfo @StudentId=''

*/
CREATE PROCEDURE [dbo].[GetDashboardInfo](
	@StudentId VARCHAR(100)=''
)
AS
BEGIN TRY
    SET NOCOUNT ON;
	BEGIN TRAN;
	
	DECLARE @TotalStudent INT=0;
	DECLARE @TotalAdmin INT=0;
	DECLARE @TotalOnlineTestExam INT=0;
	DECLARE @TotalDailyOnlineTestExam INT=0;	
	DECLARE @TotalMonthlyOnlineTestExam INT=0;	
	
	SELECT @TotalStudent=COUNT(u.Id) FROM Users u
	JOIN UserRoles ur ON u.Id=ur.UserId
	JOIN Roles r ON ur.RoleId=r.Id
	WHERE r.[Name]='Student'
	AND u.StudentId=IIF(@StudentId='', u.StudentId, @StudentId) 
	
	SELECT @TotalAdmin=COUNT(u.Id) FROM Users u
	JOIN UserRoles ur ON u.Id=ur.UserId
	JOIN Roles r ON ur.RoleId=r.Id
	WHERE r.[Name]='Admin'

	SELECT @TotalOnlineTestExam=COUNT(r.Id) FROM [dbo].[ResultSheets] r
	WHERE r.StudentId=IIF(@StudentId='', r.StudentId, @StudentId) 

	SELECT @TotalDailyOnlineTestExam=COUNT(r.Id) FROM [dbo].[ResultSheets] r
	WHERE r.StudentId=IIF(@StudentId='', r.StudentId, @StudentId)
	AND CAST(r.ExamDateTime AS DATE)=CAST(GETDATE() AS DATE)

	SELECT @TotalMonthlyOnlineTestExam=COUNT(r.Id) FROM [dbo].[ResultSheets] r
	WHERE r.StudentId=IIF(@StudentId='', r.StudentId, @StudentId) 
	AND MONTH(r.ExamDateTime)=MONTH(GETDATE())
	AND YEAR(r.ExamDateTime)=YEAR(GETDATE())

	SELECT ISNULL(@TotalAdmin,0) TotalAdmin,
	ISNULL(@TotalStudent,0) TotalStudent,
	ISNULL(@TotalOnlineTestExam,0) TotalOnlineTestExam,
	ISNULL(@TotalDailyOnlineTestExam,0) TotalDailyOnlineTestExam,
	ISNULL(@TotalMonthlyOnlineTestExam,0) TotalMonthlyOnlineTestExam
	COMMIT TRAN;
END TRY
BEGIN CATCH
      IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
      SELECT 'Error Number:' + CAST(ERROR_NUMBER() AS VARCHAR(MAX)) 
	        + ', Error State: '  + CAST(ERROR_STATE() AS VARCHAR(MAX)) 
	        + ', Error Procedure: '  + CAST(ERROR_PROCEDURE() AS VARCHAR(MAX)) 
	        + ', Error Line: '  + CAST(ERROR_LINE() AS VARCHAR(MAX)) 
	        + ', Error Message: '  + CAST(ERROR_MESSAGE() AS VARCHAR(MAX))
END CATCH
