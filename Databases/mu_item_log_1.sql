-- MySQL dump 10.13  Distrib 5.5.28, for Win64 (x86)
--
-- Host: localhost    Database: mu_item_log_1270
-- ------------------------------------------------------
-- Server version	5.5.28

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
-- Table structure for table `t_log_20150603`
--

DROP TABLE IF EXISTS `t_log_20150603`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_log_20150603` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DBId` int(11) DEFAULT NULL COMMENT 'ÎïÆ·ÔÚÎïÆ·±íµÄÊý¾Ý¿âID£¬²»ÊÇÎïÆ·Ê±IdÎª-1',
  `ObjName` char(32) DEFAULT NULL COMMENT '²Ù×÷¶ÔÏóÃû³Æ',
  `optFrom` char(32) DEFAULT NULL COMMENT '²Ù×÷²úÉúµã',
  `currEnvName` char(32) DEFAULT NULL COMMENT '¶ÔÏóËùÔÚµ±Ç°»·¾³Ãû³Æ',
  `tarEnvName` char(32) DEFAULT NULL COMMENT '¶ÔÏó½«Òªµ½´ïµÄ»·¾³Ãû³Æ',
  `optType` char(6) DEFAULT NULL COMMENT '²Ù×÷ÀàÐÍ£¬ÈçÏÂ£ºÔö¼Ó¡¢Ïú»Ù¡¢ÐÞ¸Ä¡¢ÒÆ¶¯',
  `optTime` datetime DEFAULT NULL COMMENT '²Ù×÷Ê±¼ä',
  `optAmount` int(11) DEFAULT NULL COMMENT '²Ù×÷ÊýÁ¿',
  `zoneID` int(11) DEFAULT NULL COMMENT 'Çø±àºÅ',
  `optSurplus` int(11) DEFAULT NULL COMMENT 'ÊôÐÔ²Ù×÷ºóÊ£ÓàÖµ',
  PRIMARY KEY (`Id`),
  KEY `DBId` (`DBId`),
  KEY `tarEnvName` (`tarEnvName`)
) ENGINE=MyISAM AUTO_INCREMENT=76570 DEFAULT CHARSET=utf8 COMMENT='ÎïÆ·²Ù×÷ÈÕÖ¾±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_log_20150603`
--

LOCK TABLES `t_log_20150603` WRITE;
/*!40000 ALTER TABLE `t_log_20150603` DISABLE KEYS */;
INSERT INTO `t_log_20150603` VALUES (76528,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1000','ÏµÍ³','Âí¿­Î÷¡¤Ê©ÄÍµÂ','Ôö¼Ó','2015-06-03 18:27:50',1972,1270,1972),(76529,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1000','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:36:24',1972,1270,1972),(76530,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1010','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:36:36',2268,1270,4240),(76531,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1020','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:36:50',2608,1270,6848),(76532,-1,'°ó¶¨×êÊ¯','Íê³É³É¾ÍID£º100','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','Ôö¼Ó','2015-06-03 18:36:58',20,1270,20),(76533,-1,'³É¾Í','Íê³É³É¾ÍID£º100','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÐÞ¸Ä','2015-06-03 18:36:58',50,1270,50),(76534,5,'1×¿Ô½¶¾Éß·¨ÕÈ','ÈÎÎñ½±Àø','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:37:05',1,1270,-1),(76535,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1030','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:37:05',2999,1270,9847),(76536,5,'1×¿Ô½¶¾Éß·¨ÕÈ','¿Í»§¶ËÐÞ¸Ä','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','ÐÞ¸Ä','2015-06-03 18:37:07',0,1270,-1),(76537,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1040','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:37:26',3449,1270,13296),(76538,6,'1×¿Ô½¸ïîø','ÈÎÎñ½±Àø','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:37:54',1,1270,-1),(76539,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1050','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:37:54',3966,1270,17262),(76540,6,'1×¿Ô½¸ïîø','¿Í»§¶ËÐÞ¸Ä','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','ÐÞ¸Ä','2015-06-03 18:37:58',0,1270,-1),(76541,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1051','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:38:15',4561,1270,21823),(76542,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1060','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:38:27',5245,1270,27068),(76543,7,'¸´»îÊ¯','ÈÎÎñ½±Àø','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:38:38',1,1270,-1),(76544,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1061','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:38:38',6032,1270,33100),(76545,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1070','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:38:45',6937,1270,40037),(76546,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1071','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:38:59',7977,1270,48014),(76547,8,'ÓðÃ«','ÈÎÎñ½±Àø','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:39:00',5,1270,-1),(76548,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1080','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:39:00',9174,1270,57188),(76549,8,'ÓðÃ«','ÎïÆ·Ê¹ÓÃ','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','ÐÞ¸Ä','2015-06-03 18:39:08',-1,1270,-1),(76550,8,'ÓðÃ«','ÎïÆ·Ê¹ÓÃ','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','ÐÞ¸Ä','2015-06-03 18:39:09',-1,1270,-1),(76551,8,'ÓðÃ«','ÎïÆ·Ê¹ÓÃ','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','ÐÞ¸Ä','2015-06-03 18:39:09',-1,1270,-1),(76552,8,'ÓðÃ«','ÎïÆ·Ê¹ÓÃ','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','ÐÞ¸Ä','2015-06-03 18:39:10',-1,1270,-1),(76553,8,'ÓðÃ«','ÎïÆ·Ê¹ÓÃ','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','Ïú»Ù','2015-06-03 18:39:11',-1,1270,-1),(76554,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1090','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:39:33',10550,1270,67738),(76555,9,'2×¿Ô½¸ï¿ø','ÈÎÎñ½±Àø','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:39:55',1,1270,-1),(76556,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1100','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:39:55',12132,1270,79870),(76557,10,'500000°ó¶¨½ð±Ò','Òýµ¼¸øÎïÆ·µ½²Ö¿â','ÏµÍ³','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','Ôö¼Ó','2015-06-03 18:39:55',1,1270,-1),(76558,9,'2×¿Ô½¸ï¿ø','¿Í»§¶ËÐÞ¸Ä','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼','ÏµÍ³','ÐÞ¸Ä','2015-06-03 18:39:57',0,1270,-1),(76559,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1000','ÏµÍ³','ÄÝæ«¡¤Â³','Ôö¼Ó','2015-06-03 19:57:57',1972,1270,1972),(76560,-1,'°ó¶¨×êÊ¯','Ê×´Î³äÖµËÍ°ó×ê(ÔÚÏß)','ÄÝæ«¡¤Â³','ÏµÍ³','Ôö¼Ó','2015-06-03 20:03:46',100,1270,100),(76561,-1,'×êÊ¯','GMÃüÁîÇ¿ÆÈ¸üÐÂ','ÏµÍ³','ÄÝæ«¡¤Â³','Ôö¼Ó','2015-06-03 20:03:46',10,1270,100),(76562,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1000','ÏµÍ³','AD1270','Ôö¼Ó','2015-06-03 22:31:10',1972,1270,1972),(76563,15,'500000°ó¶¨½ð±Ò','Á¬ÐøµÇÂ½½±ÀøÎïÆ·','ÏµÍ³','AD1270','Ôö¼Ó','2015-06-03 22:31:29',1,1270,-1),(76564,16,'750Ä§¾§','ÀÛ¼ÆµÇÂ½ÆÕÍ¨½±Àø','ÏµÍ³','AD1270','Ôö¼Ó','2015-06-03 22:31:32',1,1270,-1),(76565,17,'×£¸£¾§Ê¯','ÀÛ¼ÆµÇÂ½ÆÕÍ¨½±Àø','ÏµÍ³','AD1270','Ôö¼Ó','2015-06-03 22:31:32',1,1270,-1),(76566,18,'6×¿Ô½·çÖ®ÏîÁ´','ÀÛ¼ÆµÇÂ½Ö°Òµ½±Àø','ÏµÍ³','AD1270','Ôö¼Ó','2015-06-03 22:31:32',1,1270,-1),(76567,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1000','ÏµÍ³','Ë¹ÌØÀ­¡¤ÂåÅå','Ôö¼Ó','2015-06-03 22:32:22',1972,1270,1972),(76568,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1010','ÏµÍ³','Ë¹ÌØÀ­¡¤ÂåÅå','Ôö¼Ó','2015-06-03 22:32:37',2268,1270,4240),(76569,-1,'°ó½ð','Íê³ÉÈÎÎñ£º1000','ÏµÍ³','Ë¹¿¨À¼¡¤ÃÀÀû','Ôö¼Ó','2015-06-03 22:32:59',1972,1270,1972);
/*!40000 ALTER TABLE `t_log_20150603` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_log_20150604`
--

DROP TABLE IF EXISTS `t_log_20150604`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_log_20150604` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DBId` int(11) DEFAULT NULL COMMENT 'ÎïÆ·ÔÚÎïÆ·±íµÄÊý¾Ý¿âID£¬²»ÊÇÎïÆ·Ê±IdÎª-1',
  `ObjName` char(32) DEFAULT NULL COMMENT '²Ù×÷¶ÔÏóÃû³Æ',
  `optFrom` char(32) DEFAULT NULL COMMENT '²Ù×÷²úÉúµã',
  `currEnvName` char(32) DEFAULT NULL COMMENT '¶ÔÏóËùÔÚµ±Ç°»·¾³Ãû³Æ',
  `tarEnvName` char(32) DEFAULT NULL COMMENT '¶ÔÏó½«Òªµ½´ïµÄ»·¾³Ãû³Æ',
  `optType` char(6) DEFAULT NULL COMMENT '²Ù×÷ÀàÐÍ£¬ÈçÏÂ£ºÔö¼Ó¡¢Ïú»Ù¡¢ÐÞ¸Ä¡¢ÒÆ¶¯',
  `optTime` datetime DEFAULT NULL COMMENT '²Ù×÷Ê±¼ä',
  `optAmount` int(11) DEFAULT NULL COMMENT '²Ù×÷ÊýÁ¿',
  `zoneID` int(11) DEFAULT NULL COMMENT 'Çø±àºÅ',
  `optSurplus` int(11) DEFAULT NULL COMMENT 'ÊôÐÔ²Ù×÷ºóÊ£ÓàÖµ',
  PRIMARY KEY (`Id`),
  KEY `DBId` (`DBId`),
  KEY `tarEnvName` (`tarEnvName`)
) ENGINE=MyISAM AUTO_INCREMENT=76528 DEFAULT CHARSET=utf8 COMMENT='ÎïÆ·²Ù×÷ÈÕÖ¾±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_log_20150604`
--

LOCK TABLES `t_log_20150604` WRITE;
/*!40000 ALTER TABLE `t_log_20150604` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_log_20150604` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_log_20150605`
--

DROP TABLE IF EXISTS `t_log_20150605`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_log_20150605` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DBId` int(11) DEFAULT NULL COMMENT 'ÎïÆ·ÔÚÎïÆ·±íµÄÊý¾Ý¿âID£¬²»ÊÇÎïÆ·Ê±IdÎª-1',
  `ObjName` char(32) DEFAULT NULL COMMENT '²Ù×÷¶ÔÏóÃû³Æ',
  `optFrom` char(32) DEFAULT NULL COMMENT '²Ù×÷²úÉúµã',
  `currEnvName` char(32) DEFAULT NULL COMMENT '¶ÔÏóËùÔÚµ±Ç°»·¾³Ãû³Æ',
  `tarEnvName` char(32) DEFAULT NULL COMMENT '¶ÔÏó½«Òªµ½´ïµÄ»·¾³Ãû³Æ',
  `optType` char(6) DEFAULT NULL COMMENT '²Ù×÷ÀàÐÍ£¬ÈçÏÂ£ºÔö¼Ó¡¢Ïú»Ù¡¢ÐÞ¸Ä¡¢ÒÆ¶¯',
  `optTime` datetime DEFAULT NULL COMMENT '²Ù×÷Ê±¼ä',
  `optAmount` int(11) DEFAULT NULL COMMENT '²Ù×÷ÊýÁ¿',
  `zoneID` int(11) DEFAULT NULL COMMENT 'Çø±àºÅ',
  `optSurplus` int(11) DEFAULT NULL COMMENT 'ÊôÐÔ²Ù×÷ºóÊ£ÓàÖµ',
  PRIMARY KEY (`Id`),
  KEY `DBId` (`DBId`),
  KEY `tarEnvName` (`tarEnvName`)
) ENGINE=MyISAM AUTO_INCREMENT=76528 DEFAULT CHARSET=utf8 COMMENT='ÎïÆ·²Ù×÷ÈÕÖ¾±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_log_20150605`
--

LOCK TABLES `t_log_20150605` WRITE;
/*!40000 ALTER TABLE `t_log_20150605` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_log_20150605` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_log_20150606`
--

DROP TABLE IF EXISTS `t_log_20150606`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_log_20150606` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DBId` int(11) DEFAULT NULL COMMENT 'ÎïÆ·ÔÚÎïÆ·±íµÄÊý¾Ý¿âID£¬²»ÊÇÎïÆ·Ê±IdÎª-1',
  `ObjName` char(32) DEFAULT NULL COMMENT '²Ù×÷¶ÔÏóÃû³Æ',
  `optFrom` char(32) DEFAULT NULL COMMENT '²Ù×÷²úÉúµã',
  `currEnvName` char(32) DEFAULT NULL COMMENT '¶ÔÏóËùÔÚµ±Ç°»·¾³Ãû³Æ',
  `tarEnvName` char(32) DEFAULT NULL COMMENT '¶ÔÏó½«Òªµ½´ïµÄ»·¾³Ãû³Æ',
  `optType` char(6) DEFAULT NULL COMMENT '²Ù×÷ÀàÐÍ£¬ÈçÏÂ£ºÔö¼Ó¡¢Ïú»Ù¡¢ÐÞ¸Ä¡¢ÒÆ¶¯',
  `optTime` datetime DEFAULT NULL COMMENT '²Ù×÷Ê±¼ä',
  `optAmount` int(11) DEFAULT NULL COMMENT '²Ù×÷ÊýÁ¿',
  `zoneID` int(11) DEFAULT NULL COMMENT 'Çø±àºÅ',
  `optSurplus` int(11) DEFAULT NULL COMMENT 'ÊôÐÔ²Ù×÷ºóÊ£ÓàÖµ',
  PRIMARY KEY (`Id`),
  KEY `DBId` (`DBId`),
  KEY `tarEnvName` (`tarEnvName`)
) ENGINE=MyISAM AUTO_INCREMENT=76528 DEFAULT CHARSET=utf8 COMMENT='ÎïÆ·²Ù×÷ÈÕÖ¾±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_log_20150606`
--

LOCK TABLES `t_log_20150606` WRITE;
/*!40000 ALTER TABLE `t_log_20150606` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_log_20150606` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_trademoney_freq_log`
--

DROP TABLE IF EXISTS `t_trademoney_freq_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_trademoney_freq_log` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `dayid` int(11) DEFAULT NULL COMMENT 'ÈÕÆÚ±àºÅ',
  `roleid` int(11) DEFAULT NULL COMMENT '½ÇÉ«id',
  `zoneid` int(11) DEFAULT NULL COMMENT 'ÇøºÅ',
  `userid` varchar(64) CHARACTER SET latin1 DEFAULT NULL COMMENT 'ÕËºÅ',
  `rname` varchar(20) DEFAULT NULL,
  `type` int(11) DEFAULT NULL COMMENT 'Á÷Í¨ÀàÐÍ',
  `count` int(11) DEFAULT NULL COMMENT 'Á÷Í¨´ÎÊý',
  `updatetime` datetime DEFAULT NULL COMMENT '¸üÐÂÊ±¼ä',
  `level` int(11) DEFAULT NULL COMMENT '×ªÉú+µÈ¼¶',
  `regtime` datetime DEFAULT NULL COMMENT '½ÇÉ«´´½¨Ê±¼ä',
  `online_minute` int(11) DEFAULT NULL COMMENT '×ÜÔÚÏßÊ±³¤',
  `inputmoney` int(11) DEFAULT NULL COMMENT '³äÖµ×êÊ¯Êý',
  `usedmoney` int(11) DEFAULT NULL COMMENT 'ÏûºÄ×êÊ¯Êý',
  `currmoney` int(11) DEFAULT NULL COMMENT 'Ê£Óà×êÊ¯Êý',
  `ip` varchar(20) CHARACTER SET latin1 DEFAULT NULL COMMENT '×îºóÒ»´Î²Ù×÷µÄip',
  PRIMARY KEY (`id`),
  KEY `ix_updatetime` (`updatetime`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_trademoney_freq_log`
--

LOCK TABLES `t_trademoney_freq_log` WRITE;
/*!40000 ALTER TABLE `t_trademoney_freq_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_trademoney_freq_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_trademoney_num_log`
--

DROP TABLE IF EXISTS `t_trademoney_num_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_trademoney_num_log` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '×ÔÔöÖ÷¼ü',
  `type` int(11) DEFAULT NULL COMMENT 'Á÷Í¨ÀàÐÍ',
  `money` int(11) DEFAULT NULL COMMENT 'Á÷Í¨½ð¶î',
  `updatetime` datetime DEFAULT NULL COMMENT '¸üÐÂÊ±¼ä',
  `zoneid` int(11) DEFAULT NULL COMMENT 'ÇøºÅ',
  `userid1` varchar(64) CHARACTER SET latin1 DEFAULT NULL COMMENT 'ÊÕ¿î·½ÕËºÅid',
  `roleid1` int(11) DEFAULT NULL COMMENT 'ÊÕ¿î·½½ÇÉ«id',
  `rname1` varchar(20) DEFAULT NULL COMMENT 'ÊÕ¿î·½½ÇÉ«id',
  `inputmoney1` int(11) DEFAULT NULL COMMENT 'ÊÕ¿î·½³äÖµ×êÊ¯Êý',
  `usedmoney1` int(11) DEFAULT NULL COMMENT 'ÊÕ¿î·½ÏûºÄ×êÊ¯Êý',
  `currmoney1` int(11) DEFAULT NULL COMMENT 'ÊÕ¿î·½Ê£Óà×êÊ¯Êý',
  `online_minute1` int(11) DEFAULT NULL,
  `ip1` varchar(20) CHARACTER SET latin1 DEFAULT NULL COMMENT 'ÊÕ¿î·½ip',
  `userid2` varchar(64) CHARACTER SET latin1 DEFAULT NULL COMMENT '¸¶¿î·½ÕËºÅid',
  `roleid2` int(11) DEFAULT NULL COMMENT '¸¶¿î·½½ÇÉ«id',
  `rname2` varchar(20) DEFAULT NULL COMMENT '¸¶¿î·½½ÇÉ«id',
  `inputmoney2` int(11) DEFAULT NULL COMMENT '¸¶¿î·½³äÖµ×êÊ¯Êý',
  `usedmoney2` int(11) DEFAULT NULL COMMENT '¸¶¿î·½ÏûºÄ×êÊ¯Êý',
  `currmoney2` int(11) DEFAULT NULL COMMENT '¸¶¿î·½Ê£Óà×êÊ¯Êý',
  `online_minute2` int(11) DEFAULT NULL,
  `ip2` varchar(20) CHARACTER SET latin1 DEFAULT NULL COMMENT '¸¶¿î·½ip',
  PRIMARY KEY (`Id`),
  KEY `ix_updatetime` (`updatetime`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_trademoney_num_log`
--

LOCK TABLES `t_trademoney_num_log` WRITE;
/*!40000 ALTER TABLE `t_trademoney_num_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_trademoney_num_log` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-06-04  2:23:12
