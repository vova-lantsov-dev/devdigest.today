CREATE DATABASE  IF NOT EXISTS `devdigest` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */;
USE `devdigest`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: server2.agi.net.ua    Database: devdigest
-- ------------------------------------------------------
-- Server version	5.7.20-0ubuntu0.16.04.1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Category`
--

DROP TABLE IF EXISTS `Category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Category` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Category_Name_uindex` (`Name`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Channel`
--

DROP TABLE IF EXISTS `Channel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Channel` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  `Token` varchar(50) CHARACTER SET utf8 NOT NULL,
  `CategoryId` int(11) DEFAULT NULL,
  `Title` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Description` text COLLATE utf8mb4_unicode_ci,
  `Logo` varchar(256) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Channel_Name_uindex` (`Name`),
  KEY `Channel_Category_Id_fk` (`CategoryId`),
  CONSTRAINT `Channel_Category_Id_fk` FOREIGN KEY (`CategoryId`) REFERENCES `Category` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Event`
--

DROP TABLE IF EXISTS `Event`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Event` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) DEFAULT NULL,
  `CategoryId` int(11) DEFAULT '1',
  `Start` datetime DEFAULT NULL,
  `End` datetime DEFAULT NULL,
  `Image` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Title` varchar(250) CHARACTER SET utf8 NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8 NOT NULL,
  `Link` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Event_Category_Id_fk` (`CategoryId`),
  KEY `Event_User_Id_fk` (`UserId`),
  CONSTRAINT `Event_Category_Id_fk` FOREIGN KEY (`CategoryId`) REFERENCES `Category` (`Id`),
  CONSTRAINT `Event_User_Id_fk` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `FacebookPage`
--

DROP TABLE IF EXISTS `FacebookPage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `FacebookPage` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CategoryId` int(11) NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
  `Token` varchar(500) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FacebookPages_Id_uindex` (`Id`),
  UNIQUE KEY `FacebookPages_Token_uindex` (`Token`),
  KEY `FacebookPage_Category_Id_fk` (`CategoryId`),
  CONSTRAINT `FacebookPage_Category_Id_fk` FOREIGN KEY (`CategoryId`) REFERENCES `Category` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Language`
--

DROP TABLE IF EXISTS `Language`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Language` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(25) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Code` varchar(2) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Language_Id_uindex` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Publication`
--

DROP TABLE IF EXISTS `Publication`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Publication` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) DEFAULT NULL,
  `CategoryId` int(11) DEFAULT NULL,
  `Type` varchar(25) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Title` varchar(250) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Content` text COLLATE utf8mb4_unicode_ci,
  `Description` varchar(5000) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Image` varchar(250) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Link` varchar(250) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `DateTime` datetime NOT NULL,
  `Comment` varchar(1000) CHARACTER SET utf8 DEFAULT NULL,
  `EmbededPlayerCode` varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LanguageId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Publication_Link_uindex` (`Link`),
  KEY `Publication_Category_Id_fk` (`CategoryId`),
  KEY `Publication_User_Id_fk` (`UserId`),
  KEY `Publication_Language_Id_fk` (`LanguageId`),
  CONSTRAINT `Publication_Category_Id_fk` FOREIGN KEY (`CategoryId`) REFERENCES `Category` (`Id`),
  CONSTRAINT `Publication_Language_Id_fk` FOREIGN KEY (`LanguageId`) REFERENCES `Language` (`Id`),
  CONSTRAINT `Publication_User_Id_fk` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=364 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `User`
--

DROP TABLE IF EXISTS `User`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `User` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Key` varchar(36) COLLATE utf8mb4_unicode_ci NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `User_Key_uindex` (`Key`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Vacancy`
--

DROP TABLE IF EXISTS `Vacancy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Vacancy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Title` varchar(300) CHARACTER SET utf8 NOT NULL,
  `Description` varchar(5000) CHARACTER SET utf8 NOT NULL,
  `Contact` varchar(500) CHARACTER SET utf8 NOT NULL,
  `Active` bit(1) NOT NULL,
  `Date` datetime NOT NULL,
  `Content` text COLLATE utf8mb4_unicode_ci,
  `Image` varchar(500) CHARACTER SET utf8 DEFAULT NULL,
  `Url` varchar(5000) CHARACTER SET utf8 DEFAULT NULL,
  `UserId` int(11) NOT NULL,
  `CategoryId` int(11) NOT NULL,
  `Company` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LanguageId` int(11) DEFAULT '1',
  `Location` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY ` Vacancy_Id_uindex` (`Id`),
  UNIQUE KEY ` Vacancy_Image_uindex` (`Image`),
  KEY `Vacancy_User_Id_fk` (`UserId`),
  KEY `Vacancy_Category_Id_fk` (`CategoryId`),
  KEY `Vacancy_Language_Id_fk` (`LanguageId`),
  CONSTRAINT `Vacancy_Category_Id_fk` FOREIGN KEY (`CategoryId`) REFERENCES `Category` (`Id`),
  CONSTRAINT `Vacancy_Language_Id_fk` FOREIGN KEY (`LanguageId`) REFERENCES `Language` (`Id`),
  CONSTRAINT `Vacancy_User_Id_fk` FOREIGN KEY (`UserId`) REFERENCES `User` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'devdigest'
--

--
-- Dumping routines for database 'devdigest'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-01-22 12:23:43
