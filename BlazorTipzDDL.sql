-- A txt file for storing tipzdb's DDL script.
CREATE DATABASE if not EXISTS tipzdb;
USE tipzdb;


-- tipzdb.Users definition
CREATE TABLE if not EXISTS Users (
  employmentId char(6) NOT NULL,
  userName varchar(255) DEFAULT NULL,
  passwordHash longblob NOT NULL,
  passwordSalt longblob NOT NULL,
  createdAt datetime NOT NULL DEFAULT current_timestamp(),
  teamId int(11) DEFAULT NULL,
  userRole varchar(255) DEFAULT NULL,
  active tinyint(1) DEFAULT 1,
  firstTimeLogin tinyint(1) DEFAULT 1,
  PRIMARY KEY (employmentId)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


-- tipzdb.Teams definition
CREATE TABLE if not EXISTS Teams (
  teamId int(11) NOT NULL AUTO_INCREMENT,
  teamName varchar(255) DEFAULT NULL,
  teamLeader char(6) DEFAULT NULL,
  department int(11) DEFAULT NULL,
  createdAt datetime NOT NULL DEFAULT current_timestamp(),
  active tinyint(1) DEFAULT 1,
  PRIMARY KEY (teamId),
  UNIQUE KEY teamName (teamName)
) ENGINE=InnoDB AUTO_INCREMENT=0 DEFAULT CHARSET=latin1;


-- tipzdb.TeamMembers definition
CREATE TABLE if not EXISTS TeamMembers (
  userId char(6) NOT NULL,
  teamId int(11) NOT NULL,
  userTeamRole varchar(255) NOT NULL DEFAULT "Medlem",
  joinedAt datetime NOT NULL DEFAULT current_timestamp(),
  active tinyint(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (userId, teamId) 
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


-- tipzdb.Suggestions definition
CREATE TABLE if not EXISTS Suggestions (
  sugId int(11) NOT NULL AUTO_INCREMENT,
  ownerId int(11) NOT NULL,
  creatorId char(6) NOT NULL,
  assignedId char(6) DEFAULT NULL,
  completerId char(6) DEFAULT NULL,
  sugStatus int(1) NOT NULL,
  sugProgression int(1) DEFAULT 0,
  sugTitle varchar(32) NOT NULL,
  sugDesc varchar(3000) NOT NULL,
  createdAt datetime NOT NULL DEFAULT current_timestamp(),
  lastChanged timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  dueDate datetime DEFAULT NULL,
  categoryId int(3) NOT NULL,
  justDoIt tinyint(1) DEFAULT 0,
  beforeImage varchar(255) DEFAULT NULL,
  afterImage varchar(255) DEFAULT NULL,
  PRIMARY KEY (sugId)
) ENGINE=InnoDB AUTO_INCREMENT=0 DEFAULT CHARSET=latin1;


-- tipzdb.Categories definition
CREATE TABLE if not EXISTS Categories (
  catId int(3) NOT NULL,
  catName varchar(255) NOT NULL,
  PRIMARY KEY (catId)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


-- tipzdb.Departments definition
CREATE TABLE if not EXISTS Departments (
  depId int(11) NOT NULL AUTO_INCREMENT,
  depName varchar(255) NOT NULL,
  depLoc varchar(255) DEFAULT NULL,
  PRIMARY KEY (depId)
) ENGINE=InnoDB AUTO_INCREMENT=0 DEFAULT CHARSET=latin1;


--Constraint definition
ALTER TABLE Users 
ADD CONSTRAINT fk_Users_team FOREIGN KEY (teamId) REFERENCES Teams (teamId);

ALTER TABLE Teams
ADD CONSTRAINT fk_Teams_department FOREIGN KEY (department) REFERENCES Departments (depId),
ADD CONSTRAINT fk_Teams_teamLeader FOREIGN KEY (teamLeader) REFERENCES Users (employmentId);

ALTER TABLE Suggestions 
ADD CONSTRAINT fk_Suggestions_assigned FOREIGN KEY (assignedId) REFERENCES Users (employmentId),
ADD CONSTRAINT fk_Suggestions_completer FOREIGN KEY (completerId) REFERENCES Users (employmentId),
ADD CONSTRAINT fk_Suggestions_creator FOREIGN KEY (creatorId) REFERENCES Users (employmentId),
ADD CONSTRAINT fk_Suggestions_owner FOREIGN KEY (ownerId) REFERENCES Teams (teamId),
ADD CONSTRAINT fk_Suggestions_category FOREIGN KEY (categoryId) REFERENCES Categories (catId);

ALTER TABLE TeamMembers
ADD CONSTRAINT fk_TeamMembers_team FOREIGN KEY (teamId) REFERENCES Teams (teamId),
ADD CONSTRAINT fk_TeamMembers_user FOREIGN KEY (userId) REFERENCES Users (employmentId);



Insert into Users (employmentId, userName, userRole, firstTimeLogin, passwordHash, passwordSalt)
values ("000000", "SuperUser", "Admin", 0, "pqa[Èl_; S+uÑúE   FZA¯ ¥%ä} B(I½k M  ÛÀÇ  2Ýæ?Ö1ÉùQì¨;¿   ,Ìn-  ",
	"êÜ«(!dÑädÆ D  ðòg ¤g Gfa·! n3L_  ÕeäÅ õ ZWÏ×Àª hÐ ý à_ ÈíµAªÍFF Í üb   ?  ¦åC;6 ýMÓ ¦>_ÎÕqËÑaäßñ 3%ß+P  ('Â¼àR À¦ ðæ YSU<   ä ø)");


