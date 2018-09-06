ALTER TABLE dbo.t_BTS_Assignees
  ADD CONSTRAINT BugIdUserIdConstraint UNIQUE (BugId, UserId)