SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;


CREATE TABLE IF NOT EXISTS `bans` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `perm` tinyint(1) NOT NULL,
  `expiry` datetime NOT NULL,
  `comment` text NOT NULL,
  `post` int(11) NOT NULL,
  `IP` tinytext NOT NULL,
  `canview` tinyint(1) NOT NULL,
  `modname` text NOT NULL,
  `bannedon` datetime NOT NULL,
  `effective` tinyint(1) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

CREATE TABLE IF NOT EXISTS `board` (
  `ID` int(4) NOT NULL AUTO_INCREMENT,
  `type` int(4) NOT NULL,
  `parentT` int(4) DEFAULT NULL,
  `time` datetime DEFAULT NULL,
  `postername` text,
  `trip` text,
  `comment` text,
  `email` text,
  `subject` text,
  `password` text,
  `IP` text NOT NULL,
  `bumplevel` datetime DEFAULT NULL,
  `sticky` bit(1) NOT NULL,
  `ua` text,
  `posterID` text,
  `locked` bit(1) NOT NULL,
  `mta` bit(1) NOT NULL,
  `hasFile` bit(1) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=10 ;

CREATE TABLE IF NOT EXISTS `files` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `postID` int(11) NOT NULL,
  `chanbname` text NOT NULL,
  `realname` text NOT NULL,
  `size` bigint(20) NOT NULL,
  `dimension` text NOT NULL,
  `md5` text NOT NULL,
  `extension` text NOT NULL,
  `mimetype` text NOT NULL,
  `sfw` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=10 ;

CREATE TABLE IF NOT EXISTS `ioqueue` (
  `tid` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `lockedt` (
  `tid` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE IF NOT EXISTS `mods` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `username` text NOT NULL,
  `password` text NOT NULL,
  `power` text,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

CREATE TABLE IF NOT EXISTS `reports` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `postID` int(11) DEFAULT NULL,
  `reporterIP` text,
  `time` datetime DEFAULT NULL,
  `comment` text,
  `reasonID` int(11) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=2 ;
