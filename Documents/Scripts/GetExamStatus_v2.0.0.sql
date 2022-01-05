GO
/****** Object:  StoredProcedure [dbo].[GetExamStatus]    Script Date: 12/8/2021 12:42:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*

EXEC GetExamStatus @ExamId=6, @StudentId='R101'

*/
ALTER PROCEDURE [dbo].[GetExamStatus](
	@ExamId INT=0,
	@StudentId VARCHAR(500)=''
)
AS
BEGIN TRY
    SET NOCOUNT ON;
	BEGIN TRAN;
	DECLARE @TotalExamCount INT=0;
	DECLARE @TotalTodayExamCount INT=0;
	DECLARE @TotalMonthExamCount INT=0;
	DECLARE @TotalYearlyExamCount INT=0;	

	DECLARE @ActualMark DECIMAL(10,2)=0.00;
	DECLARE @ObtainMark DECIMAL(10,2)=0.00;
	DECLARE @ObtainMarkPercentage DECIMAL(10,2)=0.00;
	DECLARE @ExamDate DATETIME=GETDATE();
	DECLARE @MaxObtainMarks DECIMAL(10,2)=0.00;
	DECLARE @MaxObtainMarksPercentage DECIMAL(10,2)=0.00;
	DECLARE @AvgObtainMarks DECIMAL(10,2)=0.00;
	DECLARE @AvgObtainMarksPercentage DECIMAL(10,2)=0.00;

	SELECT @TotalExamCount=COUNT(e.Id) FROM [dbo].[Exams] e
	JOIN [dbo].[ExamWiseQuestions] q ON e.Id=q.ExamId
	WHERE e.Id=IIF(@ExamId=0, e.Id, @ExamId) 

	SELECT @TotalTodayExamCount=COUNT(e.Id) FROM [dbo].[Exams] e
	JOIN [dbo].[ExamWiseQuestions] q ON e.Id=q.ExamId
	WHERE e.Id=IIF(@ExamId=0, e.Id, @ExamId) 
	--AND MONTH(r.ExamDateTime)=MONTH(GETDATE())
	--AND MONTH(r.ExamDateTime)=MONTH(GETDATE())
	--AND YEAR(r.ExamDateTime)=YEAR(GETDATE())
	SELECT @ActualMark=SUM(rs.ActualMark),@ObtainMark=SUM(rs.ObtainMark), @ObtainMarkPercentage=CAST(SUM(rs.ObtainMark)*100/SUM(rs.ActualMark) AS DECIMAL(10,2)), @ExamDate=CAST(rs.ExamDateTime AS DATE), @StudentId=rs.StudentId FROM ResultSheets rs
	JOIN MCQQuestions q ON rs.QuestionId=q.Id
	WHERE rs.ExamId=@ExamId
	AND rs.StudentId=@StudentId 
	
	GROUP BY rs.ExamId, CAST(rs.ExamDateTime AS DATE), rs.StudentId

	SELECT 0 as Id,
	@StudentId StudentId,
	0 ExamCount,
	GETDATE() ExamDate,
	ISNULL(@ActualMark,0) ActualMark,
	ISNULL(@ObtainMark,0) ObtainMark,
	ISNULL(@ObtainMarkPercentage,0) ObtainMarkPercentage,
	ISNULL(@MaxObtainMarks,0) MaxObtainMarks, 
	ISNULL(@MaxObtainMarksPercentage,0) MaxObtainMarksPercentage,
	ISNULL(@AvgObtainMarks,0) AvgObtainMarks, 
	ISNULL(@AvgObtainMarksPercentage,0) AvgObtainMarksPercentage,
	@ExamId AS ExamId;
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