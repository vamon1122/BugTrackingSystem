ALTER TABLE t_BTS_TagTypes
  ADD CONSTRAINT TagTypeOrgConstraint UNIQUE (OrgId, Value)