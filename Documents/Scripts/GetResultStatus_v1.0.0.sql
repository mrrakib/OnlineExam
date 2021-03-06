/*

EXEC GetResultStatus @StudentId='Shohag Mia', @ExamCount=6

*/
ALTER PROCEDURE [dbo].[GetResultStatus](
	@StudentId VARCHAR(500),
	@ExamCount INT,
	@SubjectId INT=0,
	@ChapterId INT=0,
	@TopicId INT=0
)
AS
BEGIN TRY
    SET NOCOUNT ON;
	BEGIN TRAN;
	DECLARE @ActualMark DECIMAL(10,2)=0.00;
	DECLARE @ObtainMark DECIMAL(10,2)=0.00;
	DECLARE @ObtainMarkPercentage DECIMAL(10,2)=0.00;
	DECLARE @ExamDate DATETIME=GETDATE();
	DECLARE @MaxObtainMarks DECIMAL(10,2)=0.00;
	DECLARE @MaxObtainMarksPercentage DECIMAL(10,2)=0.00;
	DECLARE @AvgObtainMarks DECIMAL(10,2)=0.00;
	DECLARE @AvgObtainMarksPercentage DECIMAL(10,2)=0.00;
	CREATE TABLE #AvarageMarkInfo
	(
		AvgObtainMarks  DECIMAL(10,2), 
		AvgObtainMarksPercentage DECIMAL(10,2)  
	)

	SELECT @ActualMark=SUM(rs.ActualMark),@ObtainMark=SUM(rs.ObtainMark), @ObtainMarkPercentage=CAST(SUM(rs.ObtainMark)*100/SUM(rs.ActualMark) AS DECIMAL(10,2)), @ExamDate=CAST(rs.ExamDateTime AS DATE), @StudentId=rs.StudentId, @ExamCount=rs.ExamCount FROM ResultSheets rs
	JOIN MCQQuestions q ON rs.QuestionId=q.Id
	WHERE rs.ExamCount=@ExamCount
	AND rs.StudentId=@StudentId 
	AND q.SubjectId=IIF(@SubjectId=0, q.SubjectId, @SubjectId) 
	AND q.ChapterId=IIF(@ChapterId=0, q.ChapterId, @ChapterId) 
	AND q.TopicId=IIF(@TopicId=0, q.TopicId, @TopicId) 
	GROUP BY rs.ExamCount, CAST(rs.ExamDateTime AS DATE), rs.StudentId, rs.ExamCount

	SELECT TOP 1 @MaxObtainMarks= CAST(SUM(rs.ObtainMark) AS DECIMAL(10,2)), 
				 @MaxObtainMarksPercentage= CAST(SUM(rs.ObtainMark)*100/SUM(rs.ActualMark) AS DECIMAL(10,2)) FROM ResultSheets rs
	JOIN MCQQuestions q ON rs.QuestionId=q.Id
	WHERE q.SubjectId=IIF(@SubjectId=0, q.SubjectId, @SubjectId) 
	AND q.ChapterId=IIF(@ChapterId=0, q.ChapterId, @ChapterId) 
	AND q.TopicId=IIF(@TopicId=0, q.TopicId, @TopicId) 
	GROUP BY rs.StudentId, rs.ExamCount
	ORDER BY SUM(rs.ObtainMark) DESC

	INSERT INTO #AvarageMarkInfo
	SELECT CAST(SUM(rs.ObtainMark) AS DECIMAL(10,2)),CAST(SUM(rs.ObtainMark)*100/SUM(rs.ActualMark) AS DECIMAL(10,2)) FROM ResultSheets rs
	JOIN MCQQuestions q ON rs.QuestionId=q.Id
	WHERE q.SubjectId=IIF(@SubjectId=0, q.SubjectId, @SubjectId) 
	AND q.ChapterId=IIF(@ChapterId=0, q.ChapterId, @ChapterId) 
	AND q.TopicId=IIF(@TopicId=0, q.TopicId, @TopicId) 
	GROUP BY rs.StudentId, rs.ExamCount
	ORDER BY SUM(rs.ObtainMark) ASC

	SELECT @AvgObtainMarks=CAST(AVG(AvgObtainMarks) AS DECIMAL(10,2)), @AvgObtainMarksPercentage=CAST(AVG(AvgObtainMarksPercentage) AS DECIMAL(10,2)) FROM #AvarageMarkInfo
	If(OBJECT_ID('tempdb..#AvarageMarkInfo') Is Not Null)
	Begin
		Drop Table #AvarageMarkInfo
	End

	SELECT 0 as Id,
	@StudentId StudentId,
	@ExamCount ExamCount,
	@ExamDate ExamDate,
	ISNULL(@ActualMark,0) ActualMark,
	ISNULL(@ObtainMark,0) ObtainMark,
	ISNULL(@ObtainMarkPercentage,0) ObtainMarkPercentage,
	ISNULL(@MaxObtainMarks,0) MaxObtainMarks, 
	ISNULL(@MaxObtainMarksPercentage,0) MaxObtainMarksPercentage,
	ISNULL(@AvgObtainMarks,0) AvgObtainMarks, 
	ISNULL(@AvgObtainMarksPercentage,0) AvgObtainMarksPercentage,
	NULL AS ExamId;

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
