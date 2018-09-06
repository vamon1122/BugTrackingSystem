ALTER TABLE dbo.t_BTS_Assignees
  ADD CONSTRAINT AssigneeConstraint UNIQUE (BugId, UserId)