-- MySQL dump 10.13  Distrib 5.5.28, for Win64 (x86)
--
-- Host: localhost    Database: mu_game_1270
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
-- Table structure for table `t_adorationinfo`
--

DROP TABLE IF EXISTS `t_adorationinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_adorationinfo` (
  `roleid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `adorationroleid` int(11) NOT NULL DEFAULT '0' COMMENT '±»³ç°Ý½ÇÉ«µÄID',
  `dayid` int(11) NOT NULL COMMENT 'ÈÕÆÚ',
  PRIMARY KEY (`roleid`,`adorationroleid`,`dayid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='½ÇÉ«³ç°ÝÐÅÏ¢';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_adorationinfo`
--

LOCK TABLES `t_adorationinfo` WRITE;
/*!40000 ALTER TABLE `t_adorationinfo` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_adorationinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_baitanbuy`
--

DROP TABLE IF EXISTS `t_baitanbuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_baitanbuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `otherroleid` int(11) NOT NULL DEFAULT '0' COMMENT '¹ºÂò½ÇÉ«ID',
  `otherrname` char(32) DEFAULT NULL,
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `forgelevel` int(11) NOT NULL DEFAULT '0' COMMENT 'Ç¿»¯¼¶±ð',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·ÑÔª±¦',
  `leftyuanbao` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàÔª±¦',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  `yinliang` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ··Ç°ó¶¨½ð±Ò°ÚÌ¯³öÊÛ¼Û¸ñ',
  `left_yinliang` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£Óà½ð±Ò',
  `tax` int(11) NOT NULL DEFAULT '0' COMMENT '½»Ò×Ë°',
  `excellenceinfo` int(11) NOT NULL DEFAULT '0' COMMENT '×¿Ô½ÊôÐÔ',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='°ÚÌ¯¹ºÂò¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_baitanbuy`
--

LOCK TABLES `t_baitanbuy` WRITE;
/*!40000 ALTER TABLE `t_baitanbuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_baitanbuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_banggongbuy`
--

DROP TABLE IF EXISTS `t_banggongbuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_banggongbuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·Ñ',
  `leftbanggong` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£Óà°ï¹±',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='°ï¹±¹ºÂò¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_banggongbuy`
--

LOCK TABLES `t_banggongbuy` WRITE;
/*!40000 ALTER TABLE `t_banggongbuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_banggongbuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_banggonghist`
--

DROP TABLE IF EXISTS `t_banggonghist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_banggonghist` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `bhid` int(11) NOT NULL DEFAULT '0' COMMENT '°ï»áID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goods1num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß1ÊýÁ¿',
  `goods2num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß2ÊýÁ¿',
  `goods3num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß3ÊýÁ¿',
  `goods4num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß4ÊýÁ¿',
  `goods5num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß5ÊýÁ¿',
  `tongqian` int(11) NOT NULL DEFAULT '0' COMMENT 'Í­Ç®',
  `banggong` int(11) NOT NULL DEFAULT '0' COMMENT '°ï¹±',
  `addtime` datetime NOT NULL COMMENT '¹±Ï×Ê±¼ä',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `rid_bhid` (`bhid`,`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='°ï»á¹±Ï×¼ÇÂ¼±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_banggonghist`
--

LOCK TABLES `t_banggonghist` WRITE;
/*!40000 ALTER TABLE `t_banggonghist` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_banggonghist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_banghui`
--

DROP TABLE IF EXISTS `t_banghui`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_banghui` (
  `bhid` int(11) NOT NULL AUTO_INCREMENT COMMENT 'Á÷Ë®ID',
  `bhname` char(32) NOT NULL COMMENT '°ï»áÃû³Æ',
  `zoneid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÇøID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '°ïÖ÷½ÇÉ«ID',
  `totalnum` int(11) NOT NULL DEFAULT '0' COMMENT '°ï»áÈËÊý',
  `totallevel` int(11) NOT NULL DEFAULT '0' COMMENT '³ÉÔ±µÈ¼¶×ÜºÍ',
  `isverfiy` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÑéÖ¤',
  `bhbulletin` char(255) NOT NULL COMMENT '°ïÅÉ¹«¸æ',
  `buildtime` datetime NOT NULL COMMENT '³ÉÁ¢Ê±¼ä',
  `qiname` char(32) NOT NULL COMMENT '°ïÆìÃû³Æ',
  `qilevel` int(11) NOT NULL DEFAULT '1' COMMENT '°ïÆìµÈ¼¶',
  `goods1num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß1¸öÊý',
  `goods2num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß2¸öÊý',
  `goods3num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß3¸öÊý',
  `goods4num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß4¸öÊý',
  `goods5num` int(11) NOT NULL DEFAULT '0' COMMENT 'µÀ¾ß5¸öÊý',
  `tongqian` int(11) NOT NULL DEFAULT '0' COMMENT 'Í­Ç®¸öÊý',
  `jitan` int(11) NOT NULL DEFAULT '1' COMMENT '¼ÀÌ³',
  `junxie` int(11) NOT NULL DEFAULT '1' COMMENT '¾üÐµ',
  `guanghuan` int(11) NOT NULL DEFAULT '1' COMMENT '¹â»·',
  `isdel` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÉ¾³ý',
  `totalcombatforce` int(11) NOT NULL DEFAULT '0',
  `fubenid` int(11) NOT NULL DEFAULT '0' COMMENT '°ï»á¸±±¾µÄID',
  `fubenstate` tinyint(4) NOT NULL DEFAULT '0' COMMENT '°ï»á¸±±¾µÄ×´Ì¬',
  `openday` smallint(6) NOT NULL DEFAULT '0' COMMENT '°ï»á¸±±¾µÄ¿ªÆôÈÕÆÚ',
  `killers` char(192) NOT NULL DEFAULT '' COMMENT '°ï»á¸±±¾µÄ»÷É±ÁÐ±í',
  PRIMARY KEY (`bhid`),
  UNIQUE KEY `bhname_zoneid` (`bhname`,`zoneid`),
  KEY `rid` (`rid`)
) ENGINE=MyISAM AUTO_INCREMENT=254000000 DEFAULT CHARSET=utf8 COMMENT='°ï»á±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_banghui`
--

LOCK TABLES `t_banghui` WRITE;
/*!40000 ALTER TABLE `t_banghui` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_banghui` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_buffer`
--

DROP TABLE IF EXISTS `t_buffer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_buffer` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'Êý¾Ý¿âÁ÷Ë®ID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `bufferid` int(11) NOT NULL DEFAULT '0' COMMENT 'bufferµÄID',
  `starttime` bigint(20) NOT NULL DEFAULT '0' COMMENT '¿ªÊ¼Ê±¼ä(ÓÃÃëÊý±íÊ¾£¬ÒòÎªÉúÃü´¢±¸»áÓÃÕâ¸ö±íÊ¾×Ü´¢±¸µÄÊýÖµ)',
  `buffersecs` bigint(20) NOT NULL DEFAULT '0' COMMENT 'bufferµÄÃëÊý',
  `bufferval` bigint(11) NOT NULL DEFAULT '0' COMMENT '¶¯Ì¬±£´æµÄÖµ',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `rid_bufferid` (`rid`,`bufferid`)
) ENGINE=MyISAM AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COMMENT='½ÇÉ«buffer ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_buffer`
--

LOCK TABLES `t_buffer` WRITE;
/*!40000 ALTER TABLE `t_buffer` DISABLE KEYS */;
INSERT INTO `t_buffer` VALUES (1,254000000,34,154,0,1800),(2,254000001,34,154,0,1800),(3,254000002,34,154,0,1800),(4,254000003,34,154,0,1800),(5,254000004,34,154,0,1800),(6,254000005,34,154,0,1800),(7,254000006,34,154,0,1800);
/*!40000 ALTER TABLE `t_buffer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_bulletin`
--

DROP TABLE IF EXISTS `t_bulletin`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_bulletin` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `msgid` char(32) NOT NULL COMMENT '¹«¸æID',
  `toplaynum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×Ü¹²Ã¿´Î·¢²¼µ½¿Í»§¶Ë²¥·ÅµÄ´ÎÊý',
  `bulletintext` varchar(255) NOT NULL COMMENT '¹«¸æÄÚÈÝ',
  `bulletintime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '¹«¸æµÄ·¢²¼Ê±¼ä',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `msgid` (`msgid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÏµÍ³ÓÀ¾Ã¹«¸æ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_bulletin`
--

LOCK TABLES `t_bulletin` WRITE;
/*!40000 ALTER TABLE `t_bulletin` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_bulletin` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_chengzhu`
--

DROP TABLE IF EXISTS `t_chengzhu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_chengzhu` (
  `Id` int(11) NOT NULL DEFAULT '0',
  `bhid` int(11) NOT NULL DEFAULT '0' COMMENT '°ï»áID',
  `kicknum` int(11) NOT NULL DEFAULT '0' COMMENT '½ñÈÕÇýÖðËûÈËµÄ´ÎÊý',
  `totaltax` int(11) NOT NULL DEFAULT '0' COMMENT '×ÜµÄË°ÊÕ',
  `taxdayid` int(11) NOT NULL DEFAULT '0' COMMENT 'Ë°ÊÕµÄÈÕID',
  `todaytax` int(11) NOT NULL DEFAULT '0' COMMENT '½ñÈÕË°ÊÕ',
  `yestodaytax` int(11) NOT NULL DEFAULT '0' COMMENT '×òÈÕË°ÊÕ',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÑïÖÝ³ÇÖ÷±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_chengzhu`
--

LOCK TABLES `t_chengzhu` WRITE;
/*!40000 ALTER TABLE `t_chengzhu` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_chengzhu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_cityinfo`
--

DROP TABLE IF EXISTS `t_cityinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_cityinfo` (
  `userid` char(64) NOT NULL COMMENT 'ÓÃ»§ID',
  `dayid` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈÕID',
  `region` char(64) NOT NULL COMMENT 'Ê¡Çø',
  `cityname` char(64) NOT NULL COMMENT 'ÏØÊÐ',
  `onlinesecs` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ÔÚÏßÊ±³¤£¨Ãë£©',
  `usedmoney` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ÏûºÄµÄ×êÊ¯',
  `inputmoney` int(10) unsigned NOT NULL DEFAULT '0' COMMENT '³äÖµµÄ×êÊ¯',
  `activeval` int(10) unsigned NOT NULL DEFAULT '0' COMMENT 'ÓÃ»§»îÔ¾Öµ',
  `lastip` char(32) NOT NULL COMMENT '×îºóÒ»´ÎµÇÂ½µÄIP',
  `starttime` datetime NOT NULL COMMENT '¼ÇÂ¼´´½¨Ê±¼ä',
  `logouttime` datetime NOT NULL COMMENT '×îºóÒ»´ÎµÇ³öÊ±¼ä',
  UNIQUE KEY `userid_dayid_cityname` (`userid`,`dayid`,`cityname`),
  KEY `starttime` (`starttime`),
  KEY `userid_cityname` (`userid`,`cityname`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='³ÇÊÐÐÅÏ¢±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_cityinfo`
--

LOCK TABLES `t_cityinfo` WRITE;
/*!40000 ALTER TABLE `t_cityinfo` DISABLE KEYS */;
INSERT INTO `t_cityinfo` VALUES ('QMQJ367640',1300,'±±¾©ÊÐ','±±¾©ÊÐ',41,0,0,5,'124.127.243.74','2015-06-03 18:27:16','2015-06-03 18:27:55'),('BD388904727',1300,'±±¾©ÊÐ','±±¾©ÊÐ',408,0,0,5,'118.244.254.16','2015-06-03 18:36:12','2015-06-03 18:43:01'),('LESHI110627628',1300,'±±¾©ÊÐ','±±¾©ÊÐ',437,0,100,5,'124.127.243.74','2015-06-03 19:57:43','2015-06-03 20:05:07'),('XYMU945290',1300,'±±¾©ÊÐ','±±¾©ÊÐ',95,0,0,10,'124.127.243.74','2015-06-03 22:30:56','2015-06-03 22:32:58'),('BD304300864',1300,'±±¾©ÊÐ','±±¾©ÊÐ',54,0,0,10,'124.127.243.74','2015-06-03 22:31:19','2015-06-03 22:33:02');
/*!40000 ALTER TABLE `t_cityinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_config`
--

DROP TABLE IF EXISTS `t_config`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_config` (
  `paramname` char(32) NOT NULL COMMENT '²ÎÊýÃû³Æ',
  `paramvalue` varchar(255) NOT NULL COMMENT '²ÎÊýÖµ(×Ö·û´®,ÆäËû¸ñÊ½×Ô¼º×ª»»)',
  UNIQUE KEY `paramname` (`paramname`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÓÎÏ·ÅäÖÃ²ÎÊý±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_config`
--

LOCK TABLES `t_config` WRITE;
/*!40000 ALTER TABLE `t_config` DISABLE KEYS */;
INSERT INTO `t_config` VALUES ('anti-addiction','0'),('anti-addiction-hint','10800'),('anti-addiction-warning','18000'),('anti-addiction-restart','18000'),('big_award_id','100'),('songli_id','1'),('disable-speed-up','1'),('money-to-jifen','1'),('half_yinliang_period','0'),('speed-up-secs','45'),('low-nofall-level','100'),('up-nofall-level','100'),('force-add-shenfenzheng','0'),('ban-speed-up-minutes','0'),('move-speed-count','10'),('punish-speed-secs','10'),('keydigtreasure','1'),('hasshengxiaoguess','1'),('allowsubgold','1'),('kaifutime','2015-06-04'),('canfetchmailattachment','1'),('chat_world_level','52'),('chat_family_level','52'),('chat_team_level','52'),('chat_private_level','52'),('chat_near_level','52'),('buchangtime','2014-09-07'),('jieridaysnum','7'),('hefuwckingnum','0'),('hefutime','2013-06-09'),('hefuwcking','0'),('hefuwckingdayid','253'),('platformtype','andrid'),('kl_giftcode_u_r_l','api1.qmqj.xy.com/GetLipin/GetLipin.aspx'),('yuedutime','2013-07-24'),('yueduchoujiangstartday','2013-11-01'),('yueduchoujiangdaysnum','8'),('jieristartday','2015-04-30'),('gamedb_version','2015-05-19 11 4094'),('gameserver_version','2015-06-03 22 4269'),('kefutime','2013-09-08'),('hint-appver','20150110'),('force-update','1'),('flag_t_roles_auto_increment','200000'),('ChongJiGiftList','0,0,0,0,0'),('PKKingRole','0'),('PKKingPushMsgDayID','154'),('BattlePushMsgDayID','154'),('money-to-yuanbao','10');
/*!40000 ALTER TABLE `t_config` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_consumelog`
--

DROP TABLE IF EXISTS `t_consumelog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_consumelog` (
  `rid` int(11) NOT NULL COMMENT '½ÇÉ«id',
  `amount` int(11) NOT NULL COMMENT 'Ïû·Ñ½ð¶î',
  `ctype` int(11) NOT NULL DEFAULT '1' COMMENT 'Ïû·ÑÀàÐÍ',
  `cdate` datetime NOT NULL COMMENT 'Ïû·ÑÈÕÆÚ',
  KEY `°´ÈÕÆÚ²éÕÒ` (`rid`,`cdate`) USING HASH
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Ïû·Ñ¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_consumelog`
--

LOCK TABLES `t_consumelog` WRITE;
/*!40000 ALTER TABLE `t_consumelog` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_consumelog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_dailydata`
--

DROP TABLE IF EXISTS `t_dailydata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_dailydata` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `expdayid` int(11) NOT NULL DEFAULT '0' COMMENT '¾­ÑéÈÕID',
  `todayexp` int(11) NOT NULL DEFAULT '0' COMMENT '½ñÈÕ¾­Ñé',
  `linglidayid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÁéÁ¦ÈÕID',
  `todaylingli` int(11) NOT NULL DEFAULT '0' COMMENT '½ñÈÕÁéÁ¦',
  `killbossdayid` int(11) NOT NULL DEFAULT '0' COMMENT 'É±BOSSÈÕID',
  `todaykillboss` int(11) NOT NULL DEFAULT '0' COMMENT '½ñÈÕÉ±BOSSÊýÁ¿',
  `fubendayid` int(11) NOT NULL DEFAULT '0' COMMENT '¸±±¾Í¨¹ØÈÕID',
  `todayfubennum` int(11) NOT NULL DEFAULT '0' COMMENT '½ñÈÕÍ¨¹Ø¸±±¾',
  `wuxingdayid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎåÐÐÆæÕóµÄÈÕID',
  `wuxingnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎåÐÐÆæÕóÁìÈ¡½±ÀøÊýÁ¿',
  UNIQUE KEY `rid` (`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÈÕ³£Êý¾Ý';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_dailydata`
--

LOCK TABLES `t_dailydata` WRITE;
/*!40000 ALTER TABLE `t_dailydata` DISABLE KEYS */;
INSERT INTO `t_dailydata` VALUES (254000000,154,40,0,0,0,0,0,0,0,0),(254000001,154,36100,0,0,0,0,0,0,0,0),(254000002,154,40,0,0,0,0,0,0,0,0),(254000003,154,40,0,0,0,0,0,0,0,0),(254000005,154,90,0,0,0,0,0,0,0,0),(254000004,154,40,0,0,0,0,0,0,0,0);
/*!40000 ALTER TABLE `t_dailydata` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_dailyjingmai`
--

DROP TABLE IF EXISTS `t_dailyjingmai`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_dailyjingmai` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `jmtime` char(32) NOT NULL COMMENT '³åÑ¨µÄÈÕÆÚ',
  `jmnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­³åÑ¨µÄ´ÎÊý(¿ÉÒÔÊÇ¸ºÊý)',
  UNIQUE KEY `rid` (`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Ã¿ÈÕÒÑ¾­³åÑ¨µÄ´ÎÊý';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_dailyjingmai`
--

LOCK TABLES `t_dailyjingmai` WRITE;
/*!40000 ALTER TABLE `t_dailyjingmai` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_dailyjingmai` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_dailytasks`
--

DROP TABLE IF EXISTS `t_dailytasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_dailytasks` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `huanid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÅÜ»·µÄID',
  `rectime` char(32) NOT NULL COMMENT 'ÅÜ»·ÈÕÆÚ',
  `recnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÅÜ»·´ÎÊý',
  `taskClass` int(11) NOT NULL DEFAULT '0' COMMENT 'ÅÜ»·ÈÎÎñÀàÐÍ',
  `extdayid` int(11) NOT NULL DEFAULT '0' COMMENT '¶îÍâ´ÎÊýµÄÌìID',
  `extnum` int(11) NOT NULL DEFAULT '0' COMMENT '¶îÍâµÄ´ÎÊý',
  UNIQUE KEY `rid_taskClass` (`rid`,`taskClass`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÅÜ»·ÈÎÎñ¼ÇÂ¼±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_dailytasks`
--

LOCK TABLES `t_dailytasks` WRITE;
/*!40000 ALTER TABLE `t_dailytasks` DISABLE KEYS */;
INSERT INTO `t_dailytasks` VALUES (254000000,1,'2015-06-03',0,5,0,0),(254000001,1,'2015-06-03',0,5,0,0),(254000002,1,'2015-06-03',0,5,0,0),(254000003,1,'2015-06-03',0,5,0,0),(254000004,1,'2015-06-03',0,5,0,0),(254000005,1,'2015-06-03',0,5,0,0),(254000006,1,'2015-06-03',0,5,0,0);
/*!40000 ALTER TABLE `t_dailytasks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_dayactivityinfo`
--

DROP TABLE IF EXISTS `t_dayactivityinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_dayactivityinfo` (
  `roleid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `activityid` int(11) NOT NULL DEFAULT '0' COMMENT '»î¶¯ÀàÐÍid',
  `timeinfo` int(11) NOT NULL COMMENT 'Ê±¼ä×Ö·û´®',
  `triggercount` int(11) NOT NULL DEFAULT '0' COMMENT '´¥·¢¼ÆÊý',
  `totalpoint` bigint(20) NOT NULL DEFAULT '0',
  `lastgettime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÉÏ´Î´¥·¢Ê±¼ä',
  PRIMARY KEY (`roleid`,`activityid`),
  UNIQUE KEY `roleid_activity_timestr` (`roleid`,`activityid`,`timeinfo`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='½ÇÉ«Ã¿ÈÕ»î¶¯ÐÅÏ¢';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_dayactivityinfo`
--

LOCK TABLES `t_dayactivityinfo` WRITE;
/*!40000 ALTER TABLE `t_dayactivityinfo` DISABLE KEYS */;
INSERT INTO `t_dayactivityinfo` VALUES (254000000,1,154,0,0,'2015-06-03 18:27:55'),(254000000,2,154,0,0,'2015-06-03 18:27:55'),(254000000,3,154,0,0,'2015-06-03 18:27:55'),(254000000,4,154,0,0,'2015-06-03 18:27:55'),(254000000,5,154,0,0,'2015-06-03 18:27:55'),(254000001,1,154,0,0,'2015-06-03 18:43:01'),(254000001,2,154,0,0,'2015-06-03 18:43:01'),(254000001,3,154,0,0,'2015-06-03 18:43:01'),(254000001,4,154,0,0,'2015-06-03 18:43:01'),(254000001,5,154,0,0,'2015-06-03 18:43:01'),(254000002,1,154,0,0,'2015-06-03 20:05:07'),(254000002,2,154,0,0,'2015-06-03 20:05:07'),(254000002,3,154,0,0,'2015-06-03 20:05:07'),(254000002,4,154,0,0,'2015-06-03 20:05:07'),(254000002,5,154,0,0,'2015-06-03 20:05:07'),(254000003,1,154,0,0,'2015-06-03 22:32:10'),(254000003,2,154,0,0,'2015-06-03 22:32:10'),(254000003,3,154,0,0,'2015-06-03 22:32:10'),(254000003,4,154,0,0,'2015-06-03 22:32:10'),(254000003,5,154,0,0,'2015-06-03 22:32:10'),(254000004,1,154,0,0,'2015-06-03 22:33:02'),(254000004,2,154,0,0,'2015-06-03 22:33:02'),(254000004,3,154,0,0,'2015-06-03 22:33:02'),(254000004,4,154,0,0,'2015-06-03 22:33:02'),(254000004,5,154,0,0,'2015-06-03 22:33:02'),(254000006,1,154,0,0,'2015-06-03 22:32:30'),(254000006,2,154,0,0,'2015-06-03 22:32:30'),(254000006,3,154,0,0,'2015-06-03 22:32:30'),(254000006,4,154,0,0,'2015-06-03 22:32:30'),(254000006,5,154,0,0,'2015-06-03 22:32:30'),(254000005,1,154,0,0,'2015-06-03 22:32:58'),(254000005,2,154,0,0,'2015-06-03 22:32:58'),(254000005,3,154,0,0,'2015-06-03 22:32:58'),(254000005,4,154,0,0,'2015-06-03 22:32:58'),(254000005,5,154,0,0,'2015-06-03 22:32:58');
/*!40000 ALTER TABLE `t_dayactivityinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_djpoints`
--

DROP TABLE IF EXISTS `t_djpoints`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_djpoints` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'Á÷Ë®ID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `djpoint` int(11) NOT NULL DEFAULT '0' COMMENT '»ý·ÖÖµ',
  `total` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×ÜµÄ²ÎÕ½´ÎÊý',
  `wincnt` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'Õ½Ê¤µÄ´ÎÊý',
  `yestoday` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×òÈÕÅÅÃû',
  `lastweek` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÉÏÖÜÅÅÃû',
  `lastmonth` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÉÏÔÂÅÅÃû',
  `dayupdown` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈÕÉý½µ',
  `weekupdown` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÖÜÉý½µ',
  `monthupdown` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÔÂÉý½µ',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `rid` (`rid`),
  KEY `djpoint` (`djpoint`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='µã½«Ì¨»ý·Ö±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_djpoints`
--

LOCK TABLES `t_djpoints` WRITE;
/*!40000 ALTER TABLE `t_djpoints` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_djpoints` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_exchange1`
--

DROP TABLE IF EXISTS `t_exchange1`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_exchange1` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0',
  `goodsid` int(11) NOT NULL DEFAULT '0',
  `goodsnum` int(11) NOT NULL DEFAULT '0',
  `leftgoodsnum` int(11) NOT NULL DEFAULT '0',
  `otherroleid` int(11) NOT NULL DEFAULT '0',
  `result` char(64) NOT NULL,
  `rectime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_exchange1`
--

LOCK TABLES `t_exchange1` WRITE;
/*!40000 ALTER TABLE `t_exchange1` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_exchange1` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_exchange2`
--

DROP TABLE IF EXISTS `t_exchange2`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_exchange2` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0',
  `yinliang` int(11) NOT NULL DEFAULT '0',
  `leftyinliang` int(11) NOT NULL DEFAULT '0',
  `otherroleid` int(11) NOT NULL DEFAULT '0',
  `rectime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_exchange2`
--

LOCK TABLES `t_exchange2` WRITE;
/*!40000 ALTER TABLE `t_exchange2` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_exchange2` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_exchange3`
--

DROP TABLE IF EXISTS `t_exchange3`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_exchange3` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0',
  `yuanbao` int(11) NOT NULL DEFAULT '0',
  `leftyuanbao` int(11) NOT NULL DEFAULT '0',
  `otherroleid` int(11) NOT NULL DEFAULT '0',
  `rectime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_exchange3`
--

LOCK TABLES `t_exchange3` WRITE;
/*!40000 ALTER TABLE `t_exchange3` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_exchange3` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_fallgoods`
--

DROP TABLE IF EXISTS `t_fallgoods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_fallgoods` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0',
  `autoid` int(11) NOT NULL DEFAULT '0',
  `goodsdbid` int(11) NOT NULL DEFAULT '0',
  `goodsid` int(11) NOT NULL DEFAULT '0',
  `goodsnum` int(11) NOT NULL DEFAULT '0',
  `binding` int(11) NOT NULL DEFAULT '0',
  `quality` int(11) NOT NULL DEFAULT '0',
  `forgelevel` int(11) NOT NULL DEFAULT '0',
  `jewellist` char(128) NOT NULL,
  `mapname` char(32) NOT NULL,
  `goodsgrid` char(32) NOT NULL,
  `fromname` char(32) NOT NULL,
  `rectime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_fallgoods`
--

LOCK TABLES `t_fallgoods` WRITE;
/*!40000 ALTER TABLE `t_fallgoods` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_fallgoods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_firstcharge`
--

DROP TABLE IF EXISTS `t_firstcharge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_firstcharge` (
  `uid` char(64) NOT NULL DEFAULT '0' COMMENT 'ÓÎÏ·Æ½Ì¨ÕÊºÅ',
  `charge_info` char(128) NOT NULL COMMENT '³äÖµÐÅÏ¢',
  `notget` int(32) NOT NULL COMMENT 'Î´ÁìÈ¡°ó½ð',
  PRIMARY KEY (`uid`)
) ENGINE=MyISAM AUTO_INCREMENT=19 DEFAULT CHARSET=utf8 COMMENT='¼ÇÂ¼Ã¿¸ö³äÖµµµÊ×´Î³äÖµÐÅÏ¢';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_firstcharge`
--

LOCK TABLES `t_firstcharge` WRITE;
/*!40000 ALTER TABLE `t_firstcharge` DISABLE KEYS */;
INSERT INTO `t_firstcharge` VALUES ('LESHI110627628','-1,10',0);
/*!40000 ALTER TABLE `t_firstcharge` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_friends`
--

DROP TABLE IF EXISTS `t_friends`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_friends` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `myid` int(11) NOT NULL DEFAULT '0' COMMENT '×Ô¼ºµÄ½ÇÉ«ID',
  `otherid` int(11) NOT NULL DEFAULT '0' COMMENT '¶Ô·½µÄ½ÇÉ«ID',
  `friendType` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÓÑÒêÀàÐÍ, 0:ÅóÓÑ 1:ºÚÃûµ¥ 2:µÐÈË',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `unique_mo` (`myid`,`otherid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÅóÓÑÁÐ±í(ºÃÓÑ£¬ºÚÃûµ¥, µÐÈË)';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_friends`
--

LOCK TABLES `t_friends` WRITE;
/*!40000 ALTER TABLE `t_friends` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_friends` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_fuben`
--

DROP TABLE IF EXISTS `t_fuben`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_fuben` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«µÄID',
  `fubenid` int(11) NOT NULL DEFAULT '0' COMMENT '¸±±¾µÄID',
  `dayid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÈÕÆÚID',
  `enternum` int(11) NOT NULL DEFAULT '0' COMMENT 'µ±ÈÕ½øÈëµÄ´ÎÊý',
  `quickpasstimer` int(11) NOT NULL DEFAULT '0',
  `finishnum` int(11) NOT NULL DEFAULT '0',
  UNIQUE KEY `rid_fubenid` (`rid`,`fubenid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='¸±±¾µØÍ¼Êý¾Ý';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_fuben`
--

LOCK TABLES `t_fuben` WRITE;
/*!40000 ALTER TABLE `t_fuben` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_fuben` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_fubenhist`
--

DROP TABLE IF EXISTS `t_fubenhist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_fubenhist` (
  `fubenid` int(11) NOT NULL DEFAULT '0' COMMENT '¸±±¾µÄID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `rname` char(32) DEFAULT NULL,
  `usedsecs` int(11) NOT NULL DEFAULT '0' COMMENT 'Í¨¹ØÊ±¼ä(Ãë)',
  UNIQUE KEY `fubenid` (`fubenid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='¸±±¾´³¹Ø¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_fubenhist`
--

LOCK TABLES `t_fubenhist` WRITE;
/*!40000 ALTER TABLE `t_fubenhist` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_fubenhist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_givemoney`
--

DROP TABLE IF EXISTS `t_givemoney`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_givemoney` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0',
  `yuanbao` int(11) NOT NULL DEFAULT '0',
  `rectime` datetime NOT NULL,
  `givetype` char(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ϵͳ�����Ԫ����¼��';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_givemoney`
--

LOCK TABLES `t_givemoney` WRITE;
/*!40000 ALTER TABLE `t_givemoney` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_givemoney` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_gmmsg`
--

DROP TABLE IF EXISTS `t_gmmsg`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_gmmsg` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `msg` text,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COMMENT='��ʱGM�����';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_gmmsg`
--

LOCK TABLES `t_gmmsg` WRITE;
/*!40000 ALTER TABLE `t_gmmsg` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_gmmsg` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_goldbuy`
--

DROP TABLE IF EXISTS `t_goldbuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_goldbuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·Ñ',
  `leftgold` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£Óà½ð±Ò',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='½ð±Ò¹ºÂò¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_goldbuy`
--

LOCK TABLES `t_goldbuy` WRITE;
/*!40000 ALTER TABLE `t_goldbuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_goldbuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_goods`
--

DROP TABLE IF EXISTS `t_goods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_goods` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `isusing` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÊ¹ÓÃÖÐ',
  `forge_level` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '¶ÍÔì¼¶±ð',
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÎïÆ·¿ªÊ¼Ê¹ÓÃÊ±¼ä',
  `endtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÎïÆ·ÉÏ´ÎÊ¹ÓÃ½áÊøÊ±¼ä',
  `site` int(11) NOT NULL DEFAULT '0' COMMENT 'ËùÔÚµÄÎ»ÖÃ(0: ±³°ü, 1:²Ö¿â)',
  `quality` int(11) unsigned NOT NULL DEFAULT '1' COMMENT 'ÎïÆ·µÄÆ·ÖÊ(Ä³Ð©×°±¸»á·ÖÆ·ÖÊ£¬²»Í¬µÄÆ·ÖÊÊôÐÔ²»Í¬£¬ÓÃ»§¸Ä±äÊôÐÔºóÒª¼ÇÂ¼ÏÂÀ´)',
  `Props` char(64) NOT NULL COMMENT 'Æ·ÖÊµÄËæ»úÊôÐÔ',
  `gcount` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `origholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×Ô´ø¿×µÄÊýÁ¿',
  `rmbholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈËÃñ±Ò´ò¿×µÄÊýÁ¿',
  `jewellist` char(128) NOT NULL COMMENT 'ÏâÇ¶µÄ±¦Ê¯ÎïÆ·IDÁÐ±í',
  `binding` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñ°ó¶¨µÄÎïÆ·',
  `bagindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÔÚ ±³°üÖÐµÄÎ»ÖÃ',
  `salemoney1` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '³öÊÛµÄÍ­Ç®¼Û¸ñ',
  `saleyuanbao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '³öÊÛµÄÔª±¦µÄ¼Û¸ñ',
  `saleyinpiao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '³öÊÛµÄÒøÆ±µÄ¸öÊý',
  `addpropindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¾«¶Í¼¶±ð',
  `bornindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'Ôö¼ÓÒ»¸öÌìÉúÊôÐÔµÄ°Ù·Ö±È',
  `lucky` int(11) NOT NULL DEFAULT '0' COMMENT 'ÐÒÔËÖµ(×çÖäÖµºÏÒ»×Ö¶Î)',
  `strong` int(11) NOT NULL DEFAULT '0' COMMENT '×°±¸ÄÍ¾Ã¶È',
  `excellenceinfo` int(11) NOT NULL DEFAULT '0',
  `appendproplev` int(11) NOT NULL DEFAULT '0',
  `equipchangelife` int(11) NOT NULL DEFAULT '0',
  `washprops` varchar(256) DEFAULT NULL,
  `ehinfo` varchar(64) DEFAULT NULL COMMENT 'ÔªËØÖ®ÐÄµÄÊôÐÔÖµ',
  PRIMARY KEY (`Id`),
  KEY `rid` (`rid`)
) ENGINE=MyISAM AUTO_INCREMENT=23 DEFAULT CHARSET=utf8 COMMENT='±³°ü±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_goods`
--

LOCK TABLES `t_goods` WRITE;
/*!40000 ALTER TABLE `t_goods` DISABLE KEYS */;
INSERT INTO `t_goods` VALUES (1,254000000,1011,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,0,0,0,0,0,0,0,0,0,0,0,NULL,''),(2,254000000,1111,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,1,0,0,0,0,0,0,0,0,0,0,NULL,''),(3,254000001,1011,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,0,0,0,0,0,0,0,0,0,0,0,NULL,''),(4,254000001,1111,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,1,0,0,0,0,0,0,0,0,0,0,NULL,''),(5,254000001,1015101,1,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',1,0,0,'',1,2,0,0,0,0,0,0,17,1,0,0,NULL,''),(6,254000001,1010101,1,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',1,0,0,'',1,2,0,0,0,0,0,0,1,256,0,0,NULL,''),(7,254000001,4000,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',1,0,0,'',1,2,0,0,0,0,0,0,0,0,0,0,NULL,''),(9,254000001,1010001,1,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',1,0,0,'',1,3,0,0,0,0,0,0,0,768,0,0,NULL,''),(10,254000001,50016,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',-1000,0,'',1,0,0,'',1,0,0,0,0,0,0,0,0,0,0,0,NULL,''),(11,254000002,1011,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,0,0,0,0,0,0,0,0,0,0,0,NULL,''),(12,254000002,1111,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,1,0,0,0,0,0,0,0,0,0,0,NULL,''),(13,254000003,1011,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,0,0,0,0,0,0,0,0,0,0,0,NULL,''),(14,254000003,1111,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,1,0,0,0,0,0,0,0,0,0,0,NULL,''),(15,254000003,50016,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',1,0,0,'',1,2,0,0,0,0,0,0,0,0,0,0,NULL,''),(16,254000003,5051,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',1,0,0,'',1,3,0,0,0,0,0,0,0,0,0,0,NULL,''),(17,254000003,2001,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',1,0,0,'',1,4,0,0,0,0,0,0,0,0,0,0,NULL,''),(18,254000003,1020503,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',1,0,0,'',1,5,0,0,0,0,0,0,0,235,0,0,NULL,''),(19,254000005,1011,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,0,0,0,0,0,0,0,0,0,0,0,NULL,''),(20,254000005,1111,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,1,0,0,0,0,0,0,0,0,0,0,NULL,''),(21,254000004,1011,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,0,0,0,0,0,0,0,0,0,0,0,NULL,''),(22,254000004,1111,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',99,0,0,'',1,1,0,0,0,0,0,0,0,0,0,0,NULL,'');
/*!40000 ALTER TABLE `t_goods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_goods_bak`
--

DROP TABLE IF EXISTS `t_goods_bak`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_goods_bak` (
  `Id` int(11) NOT NULL,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '角色ID',
  `goodsid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '物品ID',
  `isusing` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT '是否使用中',
  `forge_level` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '锻造级别',
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '物品开始使用时间',
  `endtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '物品上次使用结束时间',
  `site` int(11) NOT NULL DEFAULT '0' COMMENT '所在的位置(0: 背包, 1:仓库)',
  `quality` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '物品的品质(某些装备会分品质，不同的品质属性不同，用户改变属性后要记录下来)',
  `Props` char(64) NOT NULL COMMENT '品质的随机属性',
  `gcount` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '物品数量',
  `origholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '自带孔的数量',
  `rmbholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '人民币打孔的数量',
  `jewellist` char(128) NOT NULL COMMENT '镶嵌的宝石物品ID列表',
  `binding` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '是否绑定的物品',
  `bagindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '在 背包中的位置',
  `salemoney1` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的铜钱价格',
  `saleyuanbao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的元宝的价格',
  `saleyinpiao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的银票的个数',
  `addpropindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '精锻级别',
  `bornindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '增加一个天生属性的百分比',
  `lucky` int(11) NOT NULL DEFAULT '0' COMMENT '幸运值(诅咒值合一字段)',
  `strong` int(11) NOT NULL DEFAULT '0' COMMENT '装备耐久度',
  `excellenceinfo` int(11) NOT NULL DEFAULT '0',
  `appendproplev` int(11) NOT NULL DEFAULT '0',
  `equipchangelife` int(11) NOT NULL DEFAULT '0',
  `washprops` varchar(256) DEFAULT NULL,
  `ehinfo` varchar(64) DEFAULT NULL COMMENT 'ÔªËØÖ®ÐÄµÄÊôÐÔÖµ',
  `opstate` int(11) NOT NULL DEFAULT '0' COMMENT '（备份）操作状态',
  `optime` datetime NOT NULL COMMENT '（备份）操作时间',
  `oprole` int(11) NOT NULL DEFAULT '0' COMMENT '（备份）操作人',
  KEY `idx_id` (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='物品备份表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_goods_bak`
--

LOCK TABLES `t_goods_bak` WRITE;
/*!40000 ALTER TABLE `t_goods_bak` DISABLE KEYS */;
INSERT INTO `t_goods_bak` VALUES (8,254000001,2016,0,0,'1900-01-01 12:00:00','1900-01-01 12:00:00',0,0,'',0,0,0,'',1,3,0,0,0,0,0,0,0,0,0,0,NULL,'',0,'2015-06-03 18:39:10',0);
/*!40000 ALTER TABLE `t_goods_bak` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_goods_bak_1`
--

DROP TABLE IF EXISTS `t_goods_bak_1`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_goods_bak_1` (
  `Id` int(11) NOT NULL,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '角色ID',
  `goodsid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '物品ID',
  `isusing` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT '是否使用中',
  `forge_level` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '锻造级别',
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '物品开始使用时间',
  `endtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '物品上次使用结束时间',
  `site` int(11) NOT NULL DEFAULT '0' COMMENT '所在的位置(0: 背包, 1:仓库)',
  `quality` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '物品的品质(某些装备会分品质，不同的品质属性不同，用户改变属性后要记录下来)',
  `Props` char(64) NOT NULL COMMENT '品质的随机属性',
  `gcount` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '物品数量',
  `origholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '自带孔的数量',
  `rmbholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '人民币打孔的数量',
  `jewellist` char(128) NOT NULL COMMENT '镶嵌的宝石物品ID列表',
  `binding` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '是否绑定的物品',
  `bagindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '在 背包中的位置',
  `salemoney1` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的铜钱价格',
  `saleyuanbao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的元宝的价格',
  `saleyinpiao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的银票的个数',
  `addpropindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '精锻级别',
  `bornindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '增加一个天生属性的百分比',
  `lucky` int(11) NOT NULL DEFAULT '0' COMMENT '幸运值(诅咒值合一字段)',
  `strong` int(11) NOT NULL DEFAULT '0' COMMENT '装备耐久度',
  `excellenceinfo` int(11) NOT NULL DEFAULT '0',
  `appendproplev` int(11) NOT NULL DEFAULT '0',
  `equipchangelife` int(11) NOT NULL DEFAULT '0',
  `washprops` varchar(256) DEFAULT NULL,
  `ehinfo` varchar(64) DEFAULT NULL COMMENT 'ÔªËØÖ®ÐÄµÄÊôÐÔÖµ',
  `opstate` int(11) NOT NULL DEFAULT '0' COMMENT '（备份）操作状态',
  `optime` datetime NOT NULL COMMENT '（备份）操作时间',
  `oprole` int(11) NOT NULL DEFAULT '0' COMMENT '（备份）操作人',
  KEY `idx_id` (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='物品备份表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_goods_bak_1`
--

LOCK TABLES `t_goods_bak_1` WRITE;
/*!40000 ALTER TABLE `t_goods_bak_1` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_goods_bak_1` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_goods_bak_2`
--

DROP TABLE IF EXISTS `t_goods_bak_2`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_goods_bak_2` (
  `Id` int(11) NOT NULL,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '角色ID',
  `goodsid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '物品ID',
  `isusing` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT '是否使用中',
  `forge_level` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '锻造级别',
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '物品开始使用时间',
  `endtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '物品上次使用结束时间',
  `site` int(11) NOT NULL DEFAULT '0' COMMENT '所在的位置(0: 背包, 1:仓库)',
  `quality` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '物品的品质(某些装备会分品质，不同的品质属性不同，用户改变属性后要记录下来)',
  `Props` char(64) NOT NULL COMMENT '品质的随机属性',
  `gcount` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '物品数量',
  `origholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '自带孔的数量',
  `rmbholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '人民币打孔的数量',
  `jewellist` char(128) NOT NULL COMMENT '镶嵌的宝石物品ID列表',
  `binding` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '是否绑定的物品',
  `bagindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '在 背包中的位置',
  `salemoney1` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的铜钱价格',
  `saleyuanbao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的元宝的价格',
  `saleyinpiao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '出售的银票的个数',
  `addpropindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '精锻级别',
  `bornindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '增加一个天生属性的百分比',
  `lucky` int(11) NOT NULL DEFAULT '0' COMMENT '幸运值(诅咒值合一字段)',
  `strong` int(11) NOT NULL DEFAULT '0' COMMENT '装备耐久度',
  `excellenceinfo` int(11) NOT NULL DEFAULT '0',
  `appendproplev` int(11) NOT NULL DEFAULT '0',
  `equipchangelife` int(11) NOT NULL DEFAULT '0',
  `washprops` varchar(256) DEFAULT NULL,
  `ehinfo` varchar(64) DEFAULT NULL COMMENT 'ÔªËØÖ®ÐÄµÄÊôÐÔÖµ',
  `opstate` int(11) NOT NULL DEFAULT '0' COMMENT '（备份）操作状态',
  `optime` datetime NOT NULL COMMENT '（备份）操作时间',
  `oprole` int(11) NOT NULL DEFAULT '0' COMMENT '（备份）操作人'
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='物品备份表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_goods_bak_2`
--

LOCK TABLES `t_goods_bak_2` WRITE;
/*!40000 ALTER TABLE `t_goods_bak_2` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_goods_bak_2` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_goodslimit`
--

DROP TABLE IF EXISTS `t_goodslimit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_goodslimit` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `dayid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÈÕÆÚID',
  `usednum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­Ê¹ÓÃ´ÎÊý',
  UNIQUE KEY `rid_goodsid` (`rid`,`goodsid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÎïÆ·Ê¹ÓÃÏÞÖÆ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_goodslimit`
--

LOCK TABLES `t_goodslimit` WRITE;
/*!40000 ALTER TABLE `t_goodslimit` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_goodslimit` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_goodsprops`
--

DROP TABLE IF EXISTS `t_goodsprops`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_goodsprops` (
  `id` int(10) NOT NULL COMMENT 'ÎïÆ·dbid',
  `rid` int(10) NOT NULL COMMENT '½ÇÉ«id',
  `type` int(10) NOT NULL COMMENT 'Àà±ð',
  `props` char(128) NOT NULL COMMENT 'ÊôÐÔÖµ',
  `isdel` int(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`,`rid`,`type`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=FIXED COMMENT='ÎïÆ·ÊôÐÔÀ©Õ¹±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_goodsprops`
--

LOCK TABLES `t_goodsprops` WRITE;
/*!40000 ALTER TABLE `t_goodsprops` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_goodsprops` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_hdtequan`
--

DROP TABLE IF EXISTS `t_hdtequan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_hdtequan` (
  `Id` int(11) NOT NULL DEFAULT '0',
  `tolaofangdayid` int(11) NOT NULL DEFAULT '0' COMMENT '¹ØÈëÀÎ·¿ÈÕID',
  `tolaofangnum` int(11) NOT NULL DEFAULT '0' COMMENT '¹ØÈëÀÎ·¿ÈÕ´ÎÊý',
  `offlaofangdayid` int(11) NOT NULL DEFAULT '0' COMMENT 'Àë¿ªÀÎ·¿ÈÕID',
  `offlaofangnum` int(11) NOT NULL DEFAULT '0' COMMENT 'Àë¿ªÀÎ·¿µÄ´ÎÊý',
  `bancatdayid` int(11) NOT NULL DEFAULT '0' COMMENT '½ûÑÔµÄÈÕID',
  `bancatnum` int(11) NOT NULL DEFAULT '0' COMMENT '½ûÑÔµÄ´ÎÊý',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='»ÊµÛÌØÈ¨';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_hdtequan`
--

LOCK TABLES `t_hdtequan` WRITE;
/*!40000 ALTER TABLE `t_hdtequan` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_hdtequan` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_horses`
--

DROP TABLE IF EXISTS `t_horses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_horses` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '×ÔÔö³¤ID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `horseid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×øÆïµÄID',
  `bodyid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×øÆïµÄÐÎÏóID',
  `propsNum` char(128) NOT NULL COMMENT 'ÊôÐÔµÄÇ¿»¯´ÎÊý',
  `PropsVal` char(128) NOT NULL COMMENT 'ÊôÐÔµÄÇ¿»¯Öµ',
  `addtime` datetime NOT NULL COMMENT '¿ªÊ¼ÆôÓÃµÄÊ±¼ä',
  `isdel` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÊÇÒÑ¾­É¾³ý',
  `failednum` int(11) NOT NULL DEFAULT '0' COMMENT '±¾´Î½ø½×³É¹¦Ç°ÒÑ¾­Ê§°ÜµÄ´ÎÊý',
  `temptime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÁÙÊ±ÐÒÔËµãµÄ¿ªÊ¼Ê±¼ä',
  `tempnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÁÙÊ±ÐÒÔËµã',
  `faileddayid` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê§°Ü´ÎÊýµÄÈÕID',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÐÂµÄ×øÆï±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_horses`
--

LOCK TABLES `t_horses` WRITE;
/*!40000 ALTER TABLE `t_horses` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_horses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_huodong`
--

DROP TABLE IF EXISTS `t_huodong`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_huodong` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `loginweekid` char(32) NOT NULL COMMENT 'µÇÂ¼ÖÜID',
  `logindayid` char(32) NOT NULL COMMENT 'µÇÂ¼ÈÕID',
  `loginnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÖÜÁ¬ÐøµÇÂ¼´ÎÊý',
  `newstep` int(11) NOT NULL DEFAULT '0' COMMENT '¼ûÃæÓÐÀñÁìÈ¡²½Öè',
  `steptime` datetime NOT NULL COMMENT 'ÁìÈ¡ÉÏÒ»¸ö¼ûÃæÓÐÀñ²½ÖèµÄÊ±¼ä',
  `lastmtime` int(11) NOT NULL DEFAULT '0' COMMENT 'ÉÏ¸öÔÂµÄÔÚÏßÊ±³¤',
  `curmid` char(32) NOT NULL COMMENT '±¾ÔÂµÄ±ê¼ÇID',
  `curmtime` int(11) NOT NULL DEFAULT '0' COMMENT '±¾ÔÂµÄÔÚÏßÊ±³¤',
  `songliid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­ÁìÈ¡µÄËÍÀñ»î¶¯ID',
  `logingiftstate` int(11) NOT NULL DEFAULT '0' COMMENT 'µÇÂ¼ÀñÎïµÄÁìÈ¡×´Ì¬',
  `onlinegiftstate` int(11) NOT NULL DEFAULT '0' COMMENT 'ÔÚÏßÓÐÀñµÄÁìÈ¡×´Ì¬',
  `lastlimittimehuodongid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÏÔÊ¾µÇÂ¼»î¶¯µÄID',
  `lastlimittimedayid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÏÞÊ±µÇÂ¼µÄÈÕÆÚID',
  `limittimeloginnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÏÞÊ±µÇÂ¼ÆÚ¼äÀÛ¼Æ´ÎÊý',
  `limittimegiftstate` int(11) NOT NULL DEFAULT '0' COMMENT 'ÏÞÊ±µÇÂ¼ÁìÈ¡×´Ì¬',
  `everydayonlineawardstep` int(11) NOT NULL DEFAULT '0',
  `geteverydayonlineawarddayid` int(11) NOT NULL DEFAULT '0',
  `serieslogingetawardstep` int(11) NOT NULL DEFAULT '0',
  `seriesloginawarddayid` int(11) NOT NULL DEFAULT '0',
  `seriesloginawardgoodsid` char(255) NOT NULL DEFAULT '' COMMENT 'Á¬ÐøµÇÂ½ÁìÈ¡½±ÀøµÄÁÐ±í',
  `everydayonlineawardgoodsid` char(255) NOT NULL DEFAULT '' COMMENT 'Ã¿ÈÕÔÚÏßÁìÈ¡½±ÀøµÄÁÐ±í',
  UNIQUE KEY `rid` (`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ËÍÀñ»î¶¯±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_huodong`
--

LOCK TABLES `t_huodong` WRITE;
/*!40000 ALTER TABLE `t_huodong` DISABLE KEYS */;
INSERT INTO `t_huodong` VALUES (254000000,'3','154',1,0,'2015-06-03 18:27:11',0,'6',40,0,0,0,0,0,0,0,0,154,0,0,'',''),(254000001,'3','154',1,0,'2015-06-03 18:36:06',0,'6',410,0,0,0,0,0,0,0,0,154,0,0,'',''),(254000002,'3','154',1,0,'2015-06-03 19:57:33',0,'6',440,0,0,0,0,0,0,0,0,154,0,0,'',''),(254000003,'3','154',1,0,'2015-06-03 22:30:54',0,'6',40,0,0,0,0,0,0,0,0,154,1,154,'50016,-1,-1,-1,-1,-1,-1',''),(254000004,'3','154',1,0,'2015-06-03 22:31:11',0,'6',40,0,0,0,0,0,0,0,0,154,0,0,'',''),(254000005,'3','154',1,0,'2015-06-03 22:32:14',0,'6',40,0,0,0,0,0,0,0,0,154,0,0,'',''),(254000006,'3','154',1,0,'2015-06-03 22:32:18',0,'6',10,0,0,0,0,0,0,0,0,154,0,0,'','');
/*!40000 ALTER TABLE `t_huodong` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_huodongawardrolehist`
--

DROP TABLE IF EXISTS `t_huodongawardrolehist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_huodongawardrolehist` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `zoneid` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '½ÇÉ«ÇøºÅ',
  `activitytype` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '»î¶¯ÀàÐÍ',
  `keystr` char(128) NOT NULL DEFAULT '' COMMENT '¹Ø¼ü×Ö£¬Ê¹ÓÃ»î¶¯Ê±¼ä×Ö·û´®',
  `hasgettimes` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­ÁìÈ¡´ÎÊý',
  `lastgettime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÉÏ´ÎÁìÈ¡Ê±¼ä'
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='»î¶¯½±ÀøÀúÊ·±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_huodongawardrolehist`
--

LOCK TABLES `t_huodongawardrolehist` WRITE;
/*!40000 ALTER TABLE `t_huodongawardrolehist` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_huodongawardrolehist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_huodongawarduserhist`
--

DROP TABLE IF EXISTS `t_huodongawarduserhist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_huodongawarduserhist` (
  `userid` char(64) NOT NULL DEFAULT '0' COMMENT 'ÓÃ»§ID',
  `activitytype` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '»î¶¯ÀàÐÍ',
  `keystr` char(128) NOT NULL DEFAULT '' COMMENT '¹Ø¼ü×Ö£¬Ê¹ÓÃ»î¶¯Ê±¼ä×Ö·û´®',
  `hasgettimes` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­ÁìÈ¡´ÎÊý',
  `lastgettime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÉÏ´ÎÁìÈ¡Ê±¼ä',
  UNIQUE KEY `uactkey` (`userid`,`activitytype`,`keystr`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='»î¶¯½±ÀøÀúÊ·±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_huodongawarduserhist`
--

LOCK TABLES `t_huodongawarduserhist` WRITE;
/*!40000 ALTER TABLE `t_huodongawarduserhist` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_huodongawarduserhist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_huodongpaihang`
--

DROP TABLE IF EXISTS `t_huodongpaihang`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_huodongpaihang` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `rname` char(32) DEFAULT NULL,
  `zoneid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ÇøºÅ',
  `type` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÅÅÐÐÀàÐÍ',
  `paihang` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÅÅÐÐ',
  `phvalue` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÅÅÐÐÖµ£¬ÓÃÓÚ¼ÆËãµ±Ç°ÅÅÐÐµÄÖµ£¬±ÈÈç×°±¸ÊµÁ¦µÈ',
  `paihangtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÅÅÐÐÊ±¼ä'
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='»î¶¯ÅÅÐÐ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_huodongpaihang`
--

LOCK TABLES `t_huodongpaihang` WRITE;
/*!40000 ALTER TABLE `t_huodongpaihang` DISABLE KEYS */;
INSERT INTO `t_huodongpaihang` VALUES (254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 18:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,2,2,'2015-06-03 18:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 18:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,2,2,'2015-06-03 18:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 19:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,2,2,'2015-06-03 19:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 19:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,2,2,'2015-06-03 19:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 19:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,2,2,'2015-06-03 19:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 19:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,2,2,'2015-06-03 19:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 20:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,2,2,'2015-06-03 20:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,3,2,'2015-06-03 20:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 20:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,2,2,'2015-06-03 20:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,3,2,'2015-06-03 20:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 20:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,2,2,'2015-06-03 20:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,3,2,'2015-06-03 20:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 20:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,2,2,'2015-06-03 20:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,3,2,'2015-06-03 20:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 21:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,2,2,'2015-06-03 21:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,3,2,'2015-06-03 21:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 21:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,2,2,'2015-06-03 21:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,3,2,'2015-06-03 21:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 21:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,2,2,'2015-06-03 21:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,3,2,'2015-06-03 21:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 21:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,2,2,'2015-06-03 21:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,3,2,'2015-06-03 21:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 22:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,2,2,'2015-06-03 22:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,3,2,'2015-06-03 22:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 22:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,2,2,'2015-06-03 22:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,3,2,'2015-06-03 22:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 22:46:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,5,2,3,'2015-06-03 22:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,3,2,'2015-06-03 22:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,4,2,'2015-06-03 22:46:04'),(254000003,'AD1270',1270,5,5,2,'2015-06-03 22:46:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,5,6,2,'2015-06-03 22:46:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,5,7,1,'2015-06-03 22:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 22:46:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,33,2,3,'2015-06-03 22:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,3,2,'2015-06-03 22:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,4,2,'2015-06-03 22:46:04'),(254000003,'AD1270',1270,33,5,2,'2015-06-03 22:46:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,33,6,2,'2015-06-03 22:46:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,33,7,1,'2015-06-03 22:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 23:16:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,5,2,3,'2015-06-03 23:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,3,2,'2015-06-03 23:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,4,2,'2015-06-03 23:16:04'),(254000003,'AD1270',1270,5,5,2,'2015-06-03 23:16:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,5,6,2,'2015-06-03 23:16:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,5,7,1,'2015-06-03 23:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 23:16:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,33,2,3,'2015-06-03 23:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,3,2,'2015-06-03 23:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,4,2,'2015-06-03 23:16:04'),(254000003,'AD1270',1270,33,5,2,'2015-06-03 23:16:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,33,6,2,'2015-06-03 23:16:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,33,7,1,'2015-06-03 23:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-03 23:46:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,5,2,3,'2015-06-03 23:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,3,2,'2015-06-03 23:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,4,2,'2015-06-03 23:46:04'),(254000003,'AD1270',1270,5,5,2,'2015-06-03 23:46:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,5,6,2,'2015-06-03 23:46:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,5,7,1,'2015-06-03 23:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-03 23:46:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,33,2,3,'2015-06-03 23:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,3,2,'2015-06-03 23:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,4,2,'2015-06-03 23:46:04'),(254000003,'AD1270',1270,33,5,2,'2015-06-03 23:46:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,33,6,2,'2015-06-03 23:46:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,33,7,1,'2015-06-03 23:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-04 00:16:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,5,2,3,'2015-06-04 00:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,3,2,'2015-06-04 00:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,4,2,'2015-06-04 00:16:04'),(254000003,'AD1270',1270,5,5,2,'2015-06-04 00:16:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,5,6,2,'2015-06-04 00:16:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,5,7,1,'2015-06-04 00:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-04 00:16:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,33,2,3,'2015-06-04 00:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,3,2,'2015-06-04 00:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,4,2,'2015-06-04 00:16:04'),(254000003,'AD1270',1270,33,5,2,'2015-06-04 00:16:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,33,6,2,'2015-06-04 00:16:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,33,7,1,'2015-06-04 00:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-04 00:46:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,5,2,3,'2015-06-04 00:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,3,2,'2015-06-04 00:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,4,2,'2015-06-04 00:46:04'),(254000003,'AD1270',1270,5,5,2,'2015-06-04 00:46:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,5,6,2,'2015-06-04 00:46:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,5,7,1,'2015-06-04 00:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-04 00:46:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,33,2,3,'2015-06-04 00:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,3,2,'2015-06-04 00:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,4,2,'2015-06-04 00:46:04'),(254000003,'AD1270',1270,33,5,2,'2015-06-04 00:46:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,33,6,2,'2015-06-04 00:46:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,33,7,1,'2015-06-04 00:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-04 01:16:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,5,2,3,'2015-06-04 01:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,3,2,'2015-06-04 01:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,4,2,'2015-06-04 01:16:04'),(254000003,'AD1270',1270,5,5,2,'2015-06-04 01:16:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,5,6,2,'2015-06-04 01:16:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,5,7,1,'2015-06-04 01:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-04 01:16:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,33,2,3,'2015-06-04 01:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,3,2,'2015-06-04 01:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,4,2,'2015-06-04 01:16:04'),(254000003,'AD1270',1270,33,5,2,'2015-06-04 01:16:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,33,6,2,'2015-06-04 01:16:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,33,7,1,'2015-06-04 01:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-04 01:46:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,5,2,3,'2015-06-04 01:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,3,2,'2015-06-04 01:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,4,2,'2015-06-04 01:46:04'),(254000003,'AD1270',1270,5,5,2,'2015-06-04 01:46:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,5,6,2,'2015-06-04 01:46:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,5,7,1,'2015-06-04 01:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-04 01:46:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,33,2,3,'2015-06-04 01:46:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,3,2,'2015-06-04 01:46:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,4,2,'2015-06-04 01:46:04'),(254000003,'AD1270',1270,33,5,2,'2015-06-04 01:46:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,33,6,2,'2015-06-04 01:46:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,33,7,1,'2015-06-04 01:46:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,5,1,15,'2015-06-04 02:16:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,5,2,3,'2015-06-04 02:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,5,3,2,'2015-06-04 02:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,5,4,2,'2015-06-04 02:16:04'),(254000003,'AD1270',1270,5,5,2,'2015-06-04 02:16:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,5,6,2,'2015-06-04 02:16:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,5,7,1,'2015-06-04 02:16:04'),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1270,33,1,15,'2015-06-04 02:16:04'),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',1270,33,2,3,'2015-06-04 02:16:04'),(254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1270,33,3,2,'2015-06-04 02:16:04'),(254000002,'ÄÝæ«¡¤Â³',1270,33,4,2,'2015-06-04 02:16:04'),(254000003,'AD1270',1270,33,5,2,'2015-06-04 02:16:04'),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1270,33,6,2,'2015-06-04 02:16:04'),(254000006,'¼§Âê¡¤ÂõÒ®',1270,33,7,1,'2015-06-04 02:16:04');
/*!40000 ALTER TABLE `t_huodongpaihang` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_inputhist`
--

DROP TABLE IF EXISTS `t_inputhist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_inputhist` (
  `Id` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¸üÐÂ³äÖµ¼ÇÂ¼µÄID',
  `lastid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÉÏ´ÎÉ¨ÃèµÄID',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='¸üÐÂ³äÖµ¼ÇÂ¼µÄ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_inputhist`
--

LOCK TABLES `t_inputhist` WRITE;
/*!40000 ALTER TABLE `t_inputhist` DISABLE KEYS */;
INSERT INTO `t_inputhist` VALUES (1,1);
/*!40000 ALTER TABLE `t_inputhist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_inputlog`
--

DROP TABLE IF EXISTS `t_inputlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_inputlog` (
  `Id` int(11) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `amount` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'µ¥Î»RMB',
  `u` char(64) NOT NULL DEFAULT '0' COMMENT 'ÓÎÏ·Æ½Ì¨ÕÊºÅ',
  `order_no` char(64) NOT NULL COMMENT '¶©µ¥ºÅ±ØÐëÎ¨Ò»',
  `cporder_no` char(64) NOT NULL,
  `time` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '10Î»UnixÊ±¼ä´Á£¬µ±Ç°±±¾©Ê±¼ä',
  `sign` char(32) NOT NULL COMMENT 'ÑéÖ¤Âë',
  `inputtime` datetime NOT NULL COMMENT '¼ÇÂ¼ÈÕÖ¾µÄÊ±¼ä',
  `result` char(32) NOT NULL COMMENT '¸øÆ½Ì¨·µ»ØµÄ½á¹û',
  `zoneid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÇøºÅ',
  PRIMARY KEY (`Id`),
  KEY `inputtime` (`inputtime`) USING BTREE,
  KEY `query_money` (`inputtime`,`u`,`zoneid`,`result`) USING BTREE,
  KEY `uid` (`u`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_inputlog`
--

LOCK TABLES `t_inputlog` WRITE;
/*!40000 ALTER TABLE `t_inputlog` DISABLE KEYS */;
INSERT INTO `t_inputlog` VALUES (00000000001,10,'LESHI110627628','5021','LESHI15060320030709610001001270',1433333022,'3435eec5c1feceec306e5a183c2e0d00','2015-06-03 20:03:44','success',1270);
/*!40000 ALTER TABLE `t_inputlog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_inputlog2`
--

DROP TABLE IF EXISTS `t_inputlog2`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_inputlog2` (
  `Id` int(11) unsigned zerofill NOT NULL AUTO_INCREMENT,
  `amount` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'µ¥Î»RMB',
  `u` char(64) NOT NULL DEFAULT '0' COMMENT 'ÓÎÏ·Æ½Ì¨ÕÊºÅ',
  `order_no` char(64) NOT NULL COMMENT '¶©µ¥ºÅ±ØÐëÎ¨Ò»',
  `cporder_no` char(64) NOT NULL,
  `time` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '10Î»UnixÊ±¼ä´Á£¬µ±Ç°±±¾©Ê±¼ä',
  `sign` char(32) NOT NULL COMMENT 'ÑéÖ¤Âë',
  `inputtime` datetime NOT NULL COMMENT '¼ÇÂ¼ÈÕÖ¾µÄÊ±¼ä',
  `result` char(32) NOT NULL COMMENT '¸øÆ½Ì¨·µ»ØµÄ½á¹û',
  `zoneid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÇøºÅ',
  PRIMARY KEY (`Id`),
  KEY `inputtime` (`inputtime`) USING BTREE,
  KEY `query_money` (`inputtime`,`u`,`zoneid`,`result`) USING BTREE,
  KEY `uid` (`u`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_inputlog2`
--

LOCK TABLES `t_inputlog2` WRITE;
/*!40000 ALTER TABLE `t_inputlog2` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_inputlog2` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_jingjichang`
--

DROP TABLE IF EXISTS `t_jingjichang`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_jingjichang` (
  `roleId` int(11) NOT NULL COMMENT 'Íæ¼ÒID',
  `roleName` char(32) DEFAULT NULL,
  `name` char(32) NOT NULL,
  `zoneId` int(11) NOT NULL,
  `level` int(11) NOT NULL COMMENT 'µÈ¼¶',
  `changeLiveCount` int(11) NOT NULL COMMENT '×ªÉúµÈ¼¶',
  `occupationId` int(11) NOT NULL COMMENT 'Ö°Òµ',
  `winCount` int(11) NOT NULL DEFAULT '0' COMMENT 'Á¬Ê¤´ÎÊý',
  `ranking` int(11) NOT NULL DEFAULT '-1' COMMENT 'ÅÅÃû',
  `nextRewardTime` bigint(20) NOT NULL DEFAULT '0' COMMENT 'ÉÏ´ÎÁìÈ¡½±ÀøÊ±¼ä´Á',
  `nextChallengeTime` bigint(20) NOT NULL DEFAULT '0' COMMENT 'ÉÏ´ÎÌôÕ½Ê±¼ä´Á',
  `version` int(11) NOT NULL DEFAULT '0' COMMENT '°æ±¾ºÅ',
  `baseProps` text NOT NULL COMMENT 'Íæ¼Ò»ù´¡ÊôÐÔ',
  `extProps` text NOT NULL COMMENT 'Íæ¼ÒÀ©Õ¹ÊôÐÔ',
  `equipDatas` text NOT NULL COMMENT 'Íæ¼Ò×°±¸Êý¾Ý',
  `skillDatas` text NOT NULL COMMENT 'Íæ¼Ò¼¼ÄÜ',
  `CombatForce` int(11) NOT NULL DEFAULT '0' COMMENT 'Õ½Á¦',
  `sex` tinyint(11) NOT NULL COMMENT 'ÐÔ±ð',
  PRIMARY KEY (`roleId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_jingjichang`
--

LOCK TABLES `t_jingjichang` WRITE;
/*!40000 ALTER TABLE `t_jingjichang` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_jingjichang` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_jingjichang_zhanbao`
--

DROP TABLE IF EXISTS `t_jingjichang_zhanbao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_jingjichang_zhanbao` (
  `pkId` int(11) NOT NULL AUTO_INCREMENT COMMENT 'Ö÷¼üID',
  `roleId` int(11) NOT NULL COMMENT 'Íæ¼ÒID',
  `zhanbaoType` int(11) NOT NULL COMMENT 'Õ½±¨ÀàÐÍ',
  `challengeName` char(32) DEFAULT NULL COMMENT 'ÌôÕ½Õß»ò±»ÌôÕ½ÕßÃû×Ö',
  `value` int(11) NOT NULL,
  `createTime` datetime NOT NULL,
  PRIMARY KEY (`pkId`),
  KEY `idx_t_jingjichang_zhanbao_roleId` (`roleId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_jingjichang_zhanbao`
--

LOCK TABLES `t_jingjichang_zhanbao` WRITE;
/*!40000 ALTER TABLE `t_jingjichang_zhanbao` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_jingjichang_zhanbao` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_jingmai`
--

DROP TABLE IF EXISTS `t_jingmai`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_jingmai` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `jmid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¾­ÂöµÄID',
  `jmlevel` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¾­ÂöµÄ¼¶±ð',
  `bodylevel` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '¾­Âö²ãÊý(¼¸ÖØ¾­Âö)',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `rid_jmid_bl` (`rid`,`jmid`,`bodylevel`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='½ÇÉ«¾­ÂöÏµÍ³';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_jingmai`
--

LOCK TABLES `t_jingmai` WRITE;
/*!40000 ALTER TABLE `t_jingmai` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_jingmai` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_kf_hysy_role_log`
--

DROP TABLE IF EXISTS `t_kf_hysy_role_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_kf_hysy_role_log` (
  `rid` int(11) NOT NULL,
  `day` date NOT NULL,
  `zoneid` int(11) DEFAULT '0',
  `signup_count` smallint(6) DEFAULT '0',
  `start_game_count` smallint(6) DEFAULT '0',
  `success_count` smallint(6) DEFAULT '0',
  `faild_count` smallint(6) DEFAULT '0',
  PRIMARY KEY (`rid`,`day`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_kf_hysy_role_log`
--

LOCK TABLES `t_kf_hysy_role_log` WRITE;
/*!40000 ALTER TABLE `t_kf_hysy_role_log` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_kf_hysy_role_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_kfonlineawards`
--

DROP TABLE IF EXISTS `t_kfonlineawards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_kfonlineawards` (
  `rid` int(11) NOT NULL DEFAULT '0',
  `dayid` int(11) NOT NULL DEFAULT '0',
  `yuanbao` int(11) NOT NULL DEFAULT '0',
  `totalrolenum` int(11) NOT NULL DEFAULT '0',
  `zoneid` int(11) NOT NULL DEFAULT '0',
  UNIQUE KEY `dayid_zoneid` (`dayid`,`zoneid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='�������߽�����¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_kfonlineawards`
--

LOCK TABLES `t_kfonlineawards` WRITE;
/*!40000 ALTER TABLE `t_kfonlineawards` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_kfonlineawards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_limitgoodsbuy`
--

DROP TABLE IF EXISTS `t_limitgoodsbuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_limitgoodsbuy` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `dayid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈÕÆÚID',
  `usednum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­¹ºÂòµÄÊýÁ¿',
  UNIQUE KEY `rid_goodsid` (`rid`,`goodsid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÎïÆ·ÏÞÁ¿¹ºÂò';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_limitgoodsbuy`
--

LOCK TABLES `t_limitgoodsbuy` WRITE;
/*!40000 ALTER TABLE `t_limitgoodsbuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_limitgoodsbuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_lingdi`
--

DROP TABLE IF EXISTS `t_lingdi`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_lingdi` (
  `lingdi` int(11) NOT NULL DEFAULT '0' COMMENT 'ÁìµØ±àºÅ',
  `bhid` int(11) NOT NULL DEFAULT '0' COMMENT '°ï»áID',
  `tax` int(11) NOT NULL DEFAULT '0' COMMENT 'Ë°ÂÊ',
  `takedayid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÁìÈ¡µÄÈÕID',
  `takedaynum` int(11) NOT NULL DEFAULT '0' COMMENT 'µ±ÈÕÁìÈ¡´ÎÊý',
  `yestodaytax` int(11) NOT NULL DEFAULT '0' COMMENT '×òÈÕË°ÊÕ',
  `taxdayid` int(11) NOT NULL DEFAULT '0' COMMENT 'Ë°ÊÕÈÕID',
  `todaytax` int(11) NOT NULL DEFAULT '0' COMMENT '½ñÈÕË°ÊÕ',
  `totaltax` int(11) NOT NULL DEFAULT '0' COMMENT '×ÜµÄË°ÊÕ',
  `awardfetchday` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÁìµØÃ¿ÈÕ½±ÀøÁìÈ¡ÈÕ',
  `warrequest` tinytext COMMENT '°ï»áÍõ³ÇÕ½ÕùÇëÇó×Ö¶Î',
  UNIQUE KEY `lingdi` (`lingdi`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÁìµØÕ¼Áì±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_lingdi`
--

LOCK TABLES `t_lingdi` WRITE;
/*!40000 ALTER TABLE `t_lingdi` DISABLE KEYS */;
INSERT INTO `t_lingdi` VALUES (7,0,0,0,0,0,0,0,0,0,'');
/*!40000 ALTER TABLE `t_lingdi` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_lingyu`
--

DROP TABLE IF EXISTS `t_lingyu`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_lingyu` (
  `roleid` int(11) NOT NULL COMMENT '½ÇÉ«ID',
  `type` int(11) NOT NULL COMMENT 'ôáÓðtype',
  `level` int(6) NOT NULL DEFAULT '0' COMMENT 'ôáÓðµÈ¼¶',
  `suit` int(6) NOT NULL DEFAULT '0' COMMENT 'ôáÓðÆ·½×',
  PRIMARY KEY (`roleid`,`type`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='MUôáÓð±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_lingyu`
--

LOCK TABLES `t_lingyu` WRITE;
/*!40000 ALTER TABLE `t_lingyu` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_lingyu` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_linpinma`
--

DROP TABLE IF EXISTS `t_linpinma`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_linpinma` (
  `lipinma` char(32) NOT NULL COMMENT 'ÀñÆ·Âë×Ö·û´®',
  `huodongid` int(11) NOT NULL DEFAULT '0' COMMENT '»î¶¯ID',
  `maxnum` int(11) NOT NULL DEFAULT '0' COMMENT '×î´óÊ¹ÓÃ´ÎÊý',
  `usednum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­Ê¹ÓÃ´ÎÊý',
  `ptid` int(11) NOT NULL DEFAULT '0' COMMENT 'Æ½Ì¨ID',
  `ptrepeat` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'µ¥¸öÆ½Ì¨µÄÀñÆ·ÂëÊÇ·ñÄÜÖØ¸´Ê¹ÓÃ',
  UNIQUE KEY `lipinma` (`lipinma`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÀñÆ·Âë±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_linpinma`
--

LOCK TABLES `t_linpinma` WRITE;
/*!40000 ALTER TABLE `t_linpinma` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_linpinma` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_login`
--

DROP TABLE IF EXISTS `t_login`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_login` (
  `userid` varchar(64) NOT NULL COMMENT '账号',
  `dayid` int(11) DEFAULT '0' COMMENT '天ID用于唯一索引',
  `rid` bigint(11) NOT NULL COMMENT '角色Id(可能会变，最后一次登录的)',
  `logintime` datetime DEFAULT NULL COMMENT '登陆时间',
  `logouttime` datetime DEFAULT NULL COMMENT '登出时间',
  `ip` varchar(16) DEFAULT NULL COMMENT '登陆Ip',
  `mac` varchar(30) DEFAULT NULL COMMENT 'mac',
  `zoneid` mediumint(8) DEFAULT NULL COMMENT '区组Id',
  `onlinesecs` mediumint(8) DEFAULT '0' COMMENT '当日登录时长',
  `loginnum` mediumint(8) DEFAULT '0' COMMENT '当日登录次数',
  `c1` varchar(64) DEFAULT NULL COMMENT '预留字段1',
  `c2` varchar(64) DEFAULT NULL COMMENT '预留字段2',
  UNIQUE KEY `userid_dayid_ip` (`userid`,`dayid`,`ip`),
  KEY `logintime` (`logintime`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_login`
--

LOCK TABLES `t_login` WRITE;
/*!40000 ALTER TABLE `t_login` DISABLE KEYS */;
INSERT INTO `t_login` VALUES ('QMQJ367640',1300,254000000,'2015-06-03 18:27:14','2015-06-03 18:27:55','124.127.243.74','',1270,41,1,NULL,NULL),('BD388904727',1300,254000001,'2015-06-03 18:36:12','2015-06-03 18:43:01','118.244.254.16','',1270,408,1,NULL,NULL),('LESHI110627628',1300,254000002,'2015-06-03 19:57:43','2015-06-03 20:05:07','124.127.243.74','',1270,437,2,NULL,NULL),('XYMU945290',1300,254000005,'2015-06-03 22:30:56','2015-06-03 22:32:58','124.127.243.74','',1270,95,5,NULL,NULL),('BD304300864',1300,254000004,'2015-06-03 22:31:19','2015-06-03 22:33:02','124.127.243.74','',1270,54,7,NULL,NULL);
/*!40000 ALTER TABLE `t_login` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_mail`
--

DROP TABLE IF EXISTS `t_mail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_mail` (
  `mailid` int(11) NOT NULL AUTO_INCREMENT,
  `senderrid` int(11) NOT NULL DEFAULT '0' COMMENT '·¢ËÍ½ÇÉ«ID',
  `senderrname` char(32) DEFAULT NULL,
  `sendtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '·¢ËÍÊ±¼ä',
  `receiverrid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÓÊÕ½ÇÉ«ID',
  `reveiverrname` char(32) DEFAULT NULL,
  `readtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÊÕ¼þÈËÔÄ¶ÁÊ±¼ä',
  `isread` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÒÑ¶Á',
  `mailtype` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÓÊ¼þÀàÐÍ',
  `hasfetchattachment` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÒÑ¾­ÌáÈ¡ÁË¸½¼þ(Ç®ºÍÎïÆ·)',
  `subject` varchar(100) DEFAULT NULL,
  `content` text NOT NULL COMMENT 'ÄÚÈÝ,×î¶à×Ö·ûÊýÓÉ³ÌÐòÄÚ²¿¿ØÖÆ×Ö·û',
  `yinliang` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '·¢ËÍµÄÒøÁ½',
  `tongqian` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '·¢ËÍµÄÍ­Ç®',
  `yuanbao` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '·¢ËÍµÄÔª±¦',
  PRIMARY KEY (`mailid`),
  KEY `receiverrid` (`receiverrid`)
) ENGINE=MyISAM AUTO_INCREMENT=254000000 DEFAULT CHARSET=utf8 COMMENT='ÓÊ¼þ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_mail`
--

LOCK TABLES `t_mail` WRITE;
/*!40000 ALTER TABLE `t_mail` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_mail` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_mailgoods`
--

DROP TABLE IF EXISTS `t_mailgoods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_mailgoods` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `mailid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÓÊ¼þID',
  `goodsid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `forge_level` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '¶ÍÔì¼¶±ð',
  `quality` int(11) unsigned NOT NULL DEFAULT '1' COMMENT 'ÎïÆ·µÄÆ·ÖÊ(Ä³Ð©×°±¸»á·ÖÆ·ÖÊ£¬²»Í¬µÄÆ·ÖÊÊôÐÔ²»Í¬£¬ÓÃ»§¸Ä±äÊôÐÔºóÒª¼ÇÂ¼ÏÂÀ´)',
  `Props` char(64) NOT NULL DEFAULT '' COMMENT 'Æ·ÖÊµÄËæ»úÊôÐÔ',
  `gcount` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `binding` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñ°ó¶¨µÄÎïÆ·',
  `origholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×Ô´ø¿×µÄÊýÁ¿',
  `rmbholenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈËÃñ±Ò´ò¿×µÄÊýÁ¿',
  `jewellist` char(128) NOT NULL DEFAULT '' COMMENT 'ÏâÇ¶µÄ±¦Ê¯ÎïÆ·IDÁÐ±í',
  `addpropindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¾«¶ÍÊôÐÔ',
  `bornindex` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÌìÉúÊôÐÔ',
  `lucky` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÐÒÔËÖµ',
  `strong` int(11) unsigned NOT NULL DEFAULT '0',
  `excellenceinfo` int(11) NOT NULL DEFAULT '0',
  `appendproplev` int(11) NOT NULL DEFAULT '0',
  `equipchangelife` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `mailid` (`mailid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÓÊ¼þÎïÆ·±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_mailgoods`
--

LOCK TABLES `t_mailgoods` WRITE;
/*!40000 ALTER TABLE `t_mailgoods` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_mailgoods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_mailtemp`
--

DROP TABLE IF EXISTS `t_mailtemp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_mailtemp` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `mailid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÓÊ¼þID',
  `receiverrid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÓÊ¼þ½ÓÊÕÕß½ÇÉ«ID',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=263 DEFAULT CHARSET=utf8 COMMENT='ÓÊ¼þÉ¨ÃèÁÙÊ±±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_mailtemp`
--

LOCK TABLES `t_mailtemp` WRITE;
/*!40000 ALTER TABLE `t_mailtemp` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_mailtemp` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_mallbuy`
--

DROP TABLE IF EXISTS `t_mallbuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_mallbuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·Ñ',
  `leftmoney` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàÔª±¦',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÉÌ³Ç¹ºÂò¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_mallbuy`
--

LOCK TABLES `t_mallbuy` WRITE;
/*!40000 ALTER TABLE `t_mallbuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_mallbuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_mojingexchangeinfo`
--

DROP TABLE IF EXISTS `t_mojingexchangeinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_mojingexchangeinfo` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `roleid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `exchangeid` int(11) NOT NULL DEFAULT '0' COMMENT '¶Ò»»ID',
  `exchangenum` int(11) NOT NULL DEFAULT '0' COMMENT '¶Ò»»ÊýÁ¿',
  `dayid` int(11) NOT NULL DEFAULT '0' COMMENT '¶Ò»»ÈÕÆÚID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `roleid_mojingexchange` (`roleid`,`exchangeid`,`dayid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Ä§¾§¶Ò»»ÐÅÏ¢';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_mojingexchangeinfo`
--

LOCK TABLES `t_mojingexchangeinfo` WRITE;
/*!40000 ALTER TABLE `t_mojingexchangeinfo` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_mojingexchangeinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_money`
--

DROP TABLE IF EXISTS `t_money`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_money` (
  `userid` char(64) NOT NULL DEFAULT '0',
  `money` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'Ôª±¦(ÓÎÏ·ÖÐµÄ»õ±Ò)',
  `realmoney` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÓÃ»§³äÖµµÄÇ®(RMB)',
  `giftid` int(11) NOT NULL DEFAULT '0' COMMENT '´ó½±»î¶¯ID',
  `giftjifen` int(11) NOT NULL DEFAULT '0' COMMENT '³äÖµ»ñÈ¡µÄÀñÎï»ý·Ö',
  UNIQUE KEY `userid` (`userid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÓÃ»§Ôª±¦±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_money`
--

LOCK TABLES `t_money` WRITE;
/*!40000 ALTER TABLE `t_money` DISABLE KEYS */;
INSERT INTO `t_money` VALUES ('LESHI110627628',100,10,100,10);
/*!40000 ALTER TABLE `t_money` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_npcbuy`
--

DROP TABLE IF EXISTS `t_npcbuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_npcbuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·Ñ',
  `leftmoney` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàÒøÁ½',
  `moneytype` int(11) NOT NULL DEFAULT '0' COMMENT '»õ±ÒÀàÐÍ',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Íæ¼Ò´Ónpc¹ºÂòµÄ¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_npcbuy`
--

LOCK TABLES `t_npcbuy` WRITE;
/*!40000 ALTER TABLE `t_npcbuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_npcbuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_onlinenum`
--

DROP TABLE IF EXISTS `t_onlinenum`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_onlinenum` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT 'Á÷Ë®ID',
  `num` int(11) NOT NULL DEFAULT '0' COMMENT '×ÜµÄÔÚÏßÈËÊý',
  `rectime` datetime NOT NULL COMMENT '¼ÇÂ¼µÄÊ±¼ä',
  `mapnum` char(254) DEFAULT NULL COMMENT 'µØÍ¼ÖÐÍæ¼ÒÔÚÏßÊý¾Ý',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=259 DEFAULT CHARSET=utf8 COMMENT='ÔÚÏßÈËÊý¼ÍÂ¼±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_onlinenum`
--

LOCK TABLES `t_onlinenum` WRITE;
/*!40000 ALTER TABLE `t_onlinenum` DISABLE KEYS */;
INSERT INTO `t_onlinenum` VALUES (1,0,'2015-06-03 17:48:05','1,0|2,0|3,0'),(2,0,'2015-06-03 17:50:05','1,0|2,0|3,0'),(3,0,'2015-06-03 17:52:05','1,0|2,0|3,0'),(4,0,'2015-06-03 17:54:05','1,0|2,0|3,0'),(5,0,'2015-06-03 17:56:05','1,0|2,0|3,0'),(6,0,'2015-06-03 17:58:05','1,0|2,0|3,0'),(7,0,'2015-06-03 18:00:05','1,0|2,0|3,0'),(8,0,'2015-06-03 18:02:05','1,0|2,0|3,0'),(9,0,'2015-06-03 18:04:05','1,0|2,0|3,0'),(10,0,'2015-06-03 18:06:05','1,0|2,0|3,0'),(11,0,'2015-06-03 18:08:05','1,0|2,0|3,0'),(12,0,'2015-06-03 18:10:05','1,0|2,0|3,0'),(13,0,'2015-06-03 18:12:05','1,0|2,0|3,0'),(14,0,'2015-06-03 18:14:05','1,0|2,0|3,0'),(15,0,'2015-06-03 18:16:05','1,0|2,0|3,0'),(16,0,'2015-06-03 18:18:05','1,0|2,0|3,0'),(17,0,'2015-06-03 18:20:05','1,0|2,0|3,0'),(18,0,'2015-06-03 18:22:05','1,0|2,0|3,0'),(19,0,'2015-06-03 18:24:05','1,0|2,0|3,0'),(20,0,'2015-06-03 18:26:05','1,0|2,0|3,0'),(21,0,'2015-06-03 18:28:05','1,0|2,0|3,0'),(22,0,'2015-06-03 18:30:05','1,0|2,0|3,0'),(23,0,'2015-06-03 18:32:05','1,0|2,0|3,0'),(24,0,'2015-06-03 18:34:05','1,0|2,0|3,0'),(25,0,'2015-06-03 18:36:05','1,0|2,0|3,0'),(26,1,'2015-06-03 18:38:05','1,1|2,0|3,0'),(27,1,'2015-06-03 18:40:05','1,1|2,0|3,0'),(28,1,'2015-06-03 18:42:05','1,1|2,0|3,0'),(29,0,'2015-06-03 18:44:05','1,0|2,0|3,0'),(30,0,'2015-06-03 18:46:05','1,0|2,0|3,0'),(31,0,'2015-06-03 18:48:05','1,0|2,0|3,0'),(32,0,'2015-06-03 18:50:05','1,0|2,0|3,0'),(33,0,'2015-06-03 18:52:05','1,0|2,0|3,0'),(34,0,'2015-06-03 18:54:05','1,0|2,0|3,0'),(35,0,'2015-06-03 18:56:05','1,0|2,0|3,0'),(36,0,'2015-06-03 18:58:05','1,0|2,0|3,0'),(37,0,'2015-06-03 19:00:05','1,0|2,0|3,0'),(38,0,'2015-06-03 19:02:05','1,0|2,0|3,0'),(39,0,'2015-06-03 19:04:05','1,0|2,0|3,0'),(40,0,'2015-06-03 19:06:05','1,0|2,0|3,0'),(41,0,'2015-06-03 19:08:05','1,0|2,0|3,0'),(42,0,'2015-06-03 19:10:05','1,0|2,0|3,0'),(43,0,'2015-06-03 19:12:05','1,0|2,0|3,0'),(44,0,'2015-06-03 19:14:05','1,0|2,0|3,0'),(45,0,'2015-06-03 19:16:05','1,0|2,0|3,0'),(46,0,'2015-06-03 19:18:05','1,0|2,0|3,0'),(47,0,'2015-06-03 19:20:05','1,0|2,0|3,0'),(48,0,'2015-06-03 19:22:05','1,0|2,0|3,0'),(49,0,'2015-06-03 19:24:05','1,0|2,0|3,0'),(50,0,'2015-06-03 19:26:05','1,0|2,0|3,0'),(51,0,'2015-06-03 19:28:05','1,0|2,0|3,0'),(52,0,'2015-06-03 19:30:05','1,0|2,0|3,0'),(53,0,'2015-06-03 19:32:05','1,0|2,0|3,0'),(54,0,'2015-06-03 19:34:05','1,0|2,0|3,0'),(55,0,'2015-06-03 19:36:05','1,0|2,0|3,0'),(56,0,'2015-06-03 19:38:05','1,0|2,0|3,0'),(57,0,'2015-06-03 19:40:05','1,0|2,0|3,0'),(58,0,'2015-06-03 19:42:05','1,0|2,0|3,0'),(59,0,'2015-06-03 19:44:05','1,0|2,0|3,0'),(60,0,'2015-06-03 19:46:05','1,0|2,0|3,0'),(61,0,'2015-06-03 19:48:05','1,0|2,0|3,0'),(62,0,'2015-06-03 19:50:05','1,0|2,0|3,0'),(63,0,'2015-06-03 19:52:05','1,0|2,0|3,0'),(64,0,'2015-06-03 19:54:05','1,0|2,0|3,0'),(65,0,'2015-06-03 19:56:05','1,0|2,0|3,0'),(66,1,'2015-06-03 19:58:05','1,1|2,0|3,0'),(67,1,'2015-06-03 20:00:05','1,1|2,0|3,0'),(68,1,'2015-06-03 20:02:05','1,1|2,0|3,0'),(69,1,'2015-06-03 20:04:05','1,1|2,0|3,0'),(70,0,'2015-06-03 20:06:05','1,0|2,0|3,0'),(71,0,'2015-06-03 20:08:05','1,0|2,0|3,0'),(72,0,'2015-06-03 20:10:05','1,0|2,0|3,0'),(73,0,'2015-06-03 20:12:05','1,0|2,0|3,0'),(74,0,'2015-06-03 20:14:05','1,0|2,0|3,0'),(75,0,'2015-06-03 20:16:05','1,0|2,0|3,0'),(76,0,'2015-06-03 20:18:05','1,0|2,0|3,0'),(77,0,'2015-06-03 20:20:05','1,0|2,0|3,0'),(78,0,'2015-06-03 20:22:05','1,0|2,0|3,0'),(79,0,'2015-06-03 20:24:05','1,0|2,0|3,0'),(80,0,'2015-06-03 20:26:05','1,0|2,0|3,0'),(81,0,'2015-06-03 20:28:05','1,0|2,0|3,0'),(82,0,'2015-06-03 20:30:05','1,0|2,0|3,0'),(83,0,'2015-06-03 20:32:05','1,0|2,0|3,0'),(84,0,'2015-06-03 20:34:05','1,0|2,0|3,0'),(85,0,'2015-06-03 20:36:05','1,0|2,0|3,0'),(86,0,'2015-06-03 20:38:05','1,0|2,0|3,0'),(87,0,'2015-06-03 20:40:05','1,0|2,0|3,0'),(88,0,'2015-06-03 20:42:05','1,0|2,0|3,0'),(89,0,'2015-06-03 20:44:05','1,0|2,0|3,0'),(90,0,'2015-06-03 20:46:05','1,0|2,0|3,0'),(91,0,'2015-06-03 20:48:05','1,0|2,0|3,0'),(92,0,'2015-06-03 20:50:05','1,0|2,0|3,0'),(93,0,'2015-06-03 20:52:05','1,0|2,0|3,0'),(94,0,'2015-06-03 20:54:05','1,0|2,0|3,0'),(95,0,'2015-06-03 20:56:05','1,0|2,0|3,0'),(96,0,'2015-06-03 20:58:05','1,0|2,0|3,0'),(97,0,'2015-06-03 21:00:05','1,0|2,0|3,0'),(98,0,'2015-06-03 21:02:05','1,0|2,0|3,0'),(99,0,'2015-06-03 21:04:05','1,0|2,0|3,0'),(100,0,'2015-06-03 21:06:05','1,0|2,0|3,0'),(101,0,'2015-06-03 21:08:05','1,0|2,0|3,0'),(102,0,'2015-06-03 21:10:05','1,0|2,0|3,0'),(103,0,'2015-06-03 21:12:05','1,0|2,0|3,0'),(104,0,'2015-06-03 21:14:05','1,0|2,0|3,0'),(105,0,'2015-06-03 21:16:05','1,0|2,0|3,0'),(106,0,'2015-06-03 21:18:05','1,0|2,0|3,0'),(107,0,'2015-06-03 21:20:05','1,0|2,0|3,0'),(108,0,'2015-06-03 21:22:05','1,0|2,0|3,0'),(109,0,'2015-06-03 21:24:05','1,0|2,0|3,0'),(110,0,'2015-06-03 21:26:05','1,0|2,0|3,0'),(111,0,'2015-06-03 21:28:05','1,0|2,0|3,0'),(112,0,'2015-06-03 21:30:05','1,0|2,0|3,0'),(113,0,'2015-06-03 21:32:05','1,0|2,0|3,0'),(114,0,'2015-06-03 21:34:05','1,0|2,0|3,0'),(115,0,'2015-06-03 21:36:05','1,0|2,0|3,0'),(116,0,'2015-06-03 21:38:05','1,0|2,0|3,0'),(117,0,'2015-06-03 21:40:05','1,0|2,0|3,0'),(118,0,'2015-06-03 21:42:05','1,0|2,0|3,0'),(119,0,'2015-06-03 21:44:05','1,0|2,0|3,0'),(120,0,'2015-06-03 21:46:05','1,0|2,0|3,0'),(121,0,'2015-06-03 21:48:05','1,0|2,0|3,0'),(122,0,'2015-06-03 21:50:05','1,0|2,0|3,0'),(123,0,'2015-06-03 21:52:05','1,0|2,0|3,0'),(124,0,'2015-06-03 21:54:05','1,0|2,0|3,0'),(125,0,'2015-06-03 21:56:05','1,0|2,0|3,0'),(126,0,'2015-06-03 21:58:05','1,0|2,0|3,0'),(127,0,'2015-06-03 22:00:05','1,0|2,0|3,0'),(128,0,'2015-06-03 22:02:05','1,0|2,0|3,0'),(129,0,'2015-06-03 22:04:05','1,0|2,0|3,0'),(130,0,'2015-06-03 22:06:05','1,0|2,0|3,0'),(131,0,'2015-06-03 22:08:05','1,0|2,0|3,0'),(132,0,'2015-06-03 22:10:05','1,0|2,0|3,0'),(133,0,'2015-06-03 22:12:05','1,0|2,0|3,0'),(134,0,'2015-06-03 22:14:05','1,0|2,0|3,0'),(135,0,'2015-06-03 22:16:05','1,0|2,0|3,0'),(136,0,'2015-06-03 22:18:05','1,0|2,0|3,0'),(137,0,'2015-06-03 22:20:05','1,0|2,0|3,0'),(138,0,'2015-06-03 22:22:05','1,0|2,0|3,0'),(139,0,'2015-06-03 22:24:05','1,0|2,0|3,0'),(140,0,'2015-06-03 22:26:05','1,0|2,0|3,0'),(141,0,'2015-06-03 22:28:05','1,0|2,0|3,0'),(142,0,'2015-06-03 22:30:05','1,0|2,0|3,0'),(143,0,'2015-06-03 22:32:05','1,0|2,0|3,0'),(144,0,'2015-06-03 22:34:05','1,0|2,0|3,0'),(145,0,'2015-06-03 22:36:05','1,0|2,0|3,0'),(146,0,'2015-06-03 22:38:05','1,0|2,0|3,0'),(147,0,'2015-06-03 22:40:05','1,0|2,0|3,0'),(148,0,'2015-06-03 22:42:05','1,0|2,0|3,0'),(149,0,'2015-06-03 22:44:05','1,0|2,0|3,0'),(150,0,'2015-06-03 22:46:05','1,0|2,0|3,0'),(151,0,'2015-06-03 22:48:05','1,0|2,0|3,0'),(152,0,'2015-06-03 22:50:05','1,0|2,0|3,0'),(153,0,'2015-06-03 22:52:05','1,0|2,0|3,0'),(154,0,'2015-06-03 22:54:05','1,0|2,0|3,0'),(155,0,'2015-06-03 22:56:05','1,0|2,0|3,0'),(156,0,'2015-06-03 22:58:05','1,0|2,0|3,0'),(157,0,'2015-06-03 23:00:05','1,0|2,0|3,0'),(158,0,'2015-06-03 23:02:05','1,0|2,0|3,0'),(159,0,'2015-06-03 23:04:05','1,0|2,0|3,0'),(160,0,'2015-06-03 23:06:05','1,0|2,0|3,0'),(161,0,'2015-06-03 23:08:05','1,0|2,0|3,0'),(162,0,'2015-06-03 23:10:05','1,0|2,0|3,0'),(163,0,'2015-06-03 23:12:05','1,0|2,0|3,0'),(164,0,'2015-06-03 23:14:05','1,0|2,0|3,0'),(165,0,'2015-06-03 23:16:05','1,0|2,0|3,0'),(166,0,'2015-06-03 23:18:05','1,0|2,0|3,0'),(167,0,'2015-06-03 23:20:05','1,0|2,0|3,0'),(168,0,'2015-06-03 23:22:05','1,0|2,0|3,0'),(169,0,'2015-06-03 23:24:05','1,0|2,0|3,0'),(170,0,'2015-06-03 23:26:05','1,0|2,0|3,0'),(171,0,'2015-06-03 23:28:05','1,0|2,0|3,0'),(172,0,'2015-06-03 23:30:05','1,0|2,0|3,0'),(173,0,'2015-06-03 23:32:05','1,0|2,0|3,0'),(174,0,'2015-06-03 23:34:05','1,0|2,0|3,0'),(175,0,'2015-06-03 23:36:05','1,0|2,0|3,0'),(176,0,'2015-06-03 23:38:05','1,0|2,0|3,0'),(177,0,'2015-06-03 23:40:05','1,0|2,0|3,0'),(178,0,'2015-06-03 23:42:05','1,0|2,0|3,0'),(179,0,'2015-06-03 23:44:05','1,0|2,0|3,0'),(180,0,'2015-06-03 23:46:05','1,0|2,0|3,0'),(181,0,'2015-06-03 23:48:05','1,0|2,0|3,0'),(182,0,'2015-06-03 23:50:05','1,0|2,0|3,0'),(183,0,'2015-06-03 23:52:05','1,0|2,0|3,0'),(184,0,'2015-06-03 23:54:05','1,0|2,0|3,0'),(185,0,'2015-06-03 23:56:05','1,0|2,0|3,0'),(186,0,'2015-06-03 23:58:05','1,0|2,0|3,0'),(187,0,'2015-06-04 00:00:05','1,0|2,0|3,0'),(188,0,'2015-06-04 00:02:05','1,0|2,0|3,0'),(189,0,'2015-06-04 00:04:05','1,0|2,0|3,0'),(190,0,'2015-06-04 00:06:05','1,0|2,0|3,0'),(191,0,'2015-06-04 00:08:05','1,0|2,0|3,0'),(192,0,'2015-06-04 00:10:05','1,0|2,0|3,0'),(193,0,'2015-06-04 00:12:05','1,0|2,0|3,0'),(194,0,'2015-06-04 00:14:05','1,0|2,0|3,0'),(195,0,'2015-06-04 00:16:05','1,0|2,0|3,0'),(196,0,'2015-06-04 00:18:05','1,0|2,0|3,0'),(197,0,'2015-06-04 00:20:05','1,0|2,0|3,0'),(198,0,'2015-06-04 00:22:05','1,0|2,0|3,0'),(199,0,'2015-06-04 00:24:05','1,0|2,0|3,0'),(200,0,'2015-06-04 00:26:05','1,0|2,0|3,0'),(201,0,'2015-06-04 00:28:05','1,0|2,0|3,0'),(202,0,'2015-06-04 00:30:05','1,0|2,0|3,0'),(203,0,'2015-06-04 00:32:05','1,0|2,0|3,0'),(204,0,'2015-06-04 00:34:05','1,0|2,0|3,0'),(205,0,'2015-06-04 00:36:05','1,0|2,0|3,0'),(206,0,'2015-06-04 00:38:05','1,0|2,0|3,0'),(207,0,'2015-06-04 00:40:05','1,0|2,0|3,0'),(208,0,'2015-06-04 00:42:05','1,0|2,0|3,0'),(209,0,'2015-06-04 00:44:05','1,0|2,0|3,0'),(210,0,'2015-06-04 00:46:05','1,0|2,0|3,0'),(211,0,'2015-06-04 00:48:05','1,0|2,0|3,0'),(212,0,'2015-06-04 00:50:05','1,0|2,0|3,0'),(213,0,'2015-06-04 00:52:05','1,0|2,0|3,0'),(214,0,'2015-06-04 00:54:05','1,0|2,0|3,0'),(215,0,'2015-06-04 00:56:05','1,0|2,0|3,0'),(216,0,'2015-06-04 00:58:05','1,0|2,0|3,0'),(217,0,'2015-06-04 01:00:05','1,0|2,0|3,0'),(218,0,'2015-06-04 01:02:05','1,0|2,0|3,0'),(219,0,'2015-06-04 01:04:05','1,0|2,0|3,0'),(220,0,'2015-06-04 01:06:05','1,0|2,0|3,0'),(221,0,'2015-06-04 01:08:05','1,0|2,0|3,0'),(222,0,'2015-06-04 01:10:05','1,0|2,0|3,0'),(223,0,'2015-06-04 01:12:05','1,0|2,0|3,0'),(224,0,'2015-06-04 01:14:05','1,0|2,0|3,0'),(225,0,'2015-06-04 01:16:05','1,0|2,0|3,0'),(226,0,'2015-06-04 01:18:05','1,0|2,0|3,0'),(227,0,'2015-06-04 01:20:05','1,0|2,0|3,0'),(228,0,'2015-06-04 01:22:05','1,0|2,0|3,0'),(229,0,'2015-06-04 01:24:05','1,0|2,0|3,0'),(230,0,'2015-06-04 01:26:05','1,0|2,0|3,0'),(231,0,'2015-06-04 01:28:05','1,0|2,0|3,0'),(232,0,'2015-06-04 01:30:05','1,0|2,0|3,0'),(233,0,'2015-06-04 01:32:05','1,0|2,0|3,0'),(234,0,'2015-06-04 01:34:05','1,0|2,0|3,0'),(235,0,'2015-06-04 01:36:05','1,0|2,0|3,0'),(236,0,'2015-06-04 01:38:05','1,0|2,0|3,0'),(237,0,'2015-06-04 01:40:05','1,0|2,0|3,0'),(238,0,'2015-06-04 01:42:05','1,0|2,0|3,0'),(239,0,'2015-06-04 01:44:05','1,0|2,0|3,0'),(240,0,'2015-06-04 01:46:05','1,0|2,0|3,0'),(241,0,'2015-06-04 01:48:05','1,0|2,0|3,0'),(242,0,'2015-06-04 01:50:05','1,0|2,0|3,0'),(243,0,'2015-06-04 01:52:05','1,0|2,0|3,0'),(244,0,'2015-06-04 01:54:05','1,0|2,0|3,0'),(245,0,'2015-06-04 01:56:05','1,0|2,0|3,0'),(246,0,'2015-06-04 01:58:05','1,0|2,0|3,0'),(247,0,'2015-06-04 02:00:05','1,0|2,0|3,0'),(248,0,'2015-06-04 02:02:05','1,0|2,0|3,0'),(249,0,'2015-06-04 02:04:05','1,0|2,0|3,0'),(250,0,'2015-06-04 02:06:05','1,0|2,0|3,0'),(251,0,'2015-06-04 02:08:05','1,0|2,0|3,0'),(252,0,'2015-06-04 02:10:05','1,0|2,0|3,0'),(253,0,'2015-06-04 02:12:05','1,0|2,0|3,0'),(254,0,'2015-06-04 02:14:05','1,0|2,0|3,0'),(255,0,'2015-06-04 02:16:05','1,0|2,0|3,0'),(256,0,'2015-06-04 02:18:05','1,0|2,0|3,0'),(257,0,'2015-06-04 02:20:05','1,0|2,0|3,0'),(258,0,'2015-06-04 02:22:05','1,0|2,0|3,0');
/*!40000 ALTER TABLE `t_onlinenum` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_order`
--

DROP TABLE IF EXISTS `t_order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_order` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `order_no` char(64) NOT NULL COMMENT '¶©µ¥ºÅ',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `order_no` (`order_no`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='·ÀÖ¹¼ÇÂ¼³äÖµ¼ÇÂ¼ÖØ¸´µÄ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_order`
--

LOCK TABLES `t_order` WRITE;
/*!40000 ALTER TABLE `t_order` DISABLE KEYS */;
INSERT INTO `t_order` VALUES (1,'5021');
/*!40000 ALTER TABLE `t_order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_pets`
--

DROP TABLE IF EXISTS `t_pets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_pets` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '×ÔÔö³¤ID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `petid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '³èÎïID',
  `petname` char(32) NOT NULL COMMENT '³èÎïÃû³Æ',
  `pettype` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '³èÎïÀàÐÍ',
  `feednum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'Î¹Ê³µÄ´ÎÊý',
  `realivenum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¸´»îµÄ´ÎÊý',
  `addtime` datetime NOT NULL COMMENT 'ÁìÑø¿ªÊ¼µÄÊ±¼ä',
  `props` char(255) NOT NULL COMMENT 'À©Õ¹ÊôÐÔ',
  `isdel` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÒÑ¾­É¾³ý',
  `level` int(11) unsigned NOT NULL DEFAULT '1' COMMENT '³èÎïµÄ¼¶±ð',
  PRIMARY KEY (`Id`),
  KEY `rid` (`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='³èÎï±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_pets`
--

LOCK TABLES `t_pets` WRITE;
/*!40000 ALTER TABLE `t_pets` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_pets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_picturejudgeinfo`
--

DROP TABLE IF EXISTS `t_picturejudgeinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_picturejudgeinfo` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `roleid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `picturejudgeid` int(11) NOT NULL DEFAULT '0' COMMENT 'Í¼¼øid',
  `refercount` int(11) NOT NULL DEFAULT '0' COMMENT 'Ìá½»ÊýÁ¿',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `roleid_picturejudge` (`roleid`,`picturejudgeid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='½ÇÉ«Í¼¼øÐÅÏ¢';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_picturejudgeinfo`
--

LOCK TABLES `t_picturejudgeinfo` WRITE;
/*!40000 ALTER TABLE `t_picturejudgeinfo` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_picturejudgeinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_prenames`
--

DROP TABLE IF EXISTS `t_prenames`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_prenames` (
  `name` char(32) NOT NULL COMMENT '½ÇÉ«Ãû×Ö',
  `sex` tinyint(6) NOT NULL DEFAULT '0' COMMENT 'ÐÔ±ð, 0:ÄÐ, 1:Å®',
  `used` tinyint(6) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÒÑ¾­±»Ê¹ÓÃ',
  UNIQUE KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Ô¤ÏÈ´æ´¢µÄ½ÇÉ«Ãû×Ö';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_prenames`
--

LOCK TABLES `t_prenames` WRITE;
/*!40000 ALTER TABLE `t_prenames` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_prenames` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_ptbag`
--

DROP TABLE IF EXISTS `t_ptbag`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_ptbag` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `extgridnum` int(11) NOT NULL DEFAULT '0' COMMENT 'À©Õ¹µÄ¸ñ×Ó¸öÊý',
  UNIQUE KEY `rid` (`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ËæÉí²Ö¿âÅäÖÃ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_ptbag`
--

LOCK TABLES `t_ptbag` WRITE;
/*!40000 ALTER TABLE `t_ptbag` DISABLE KEYS */;
INSERT INTO `t_ptbag` VALUES (254000000,60),(254000001,60),(254000002,60),(254000003,60),(254000004,60),(254000005,60),(254000006,60);
/*!40000 ALTER TABLE `t_ptbag` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_pushmessageinfo`
--

DROP TABLE IF EXISTS `t_pushmessageinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_pushmessageinfo` (
  `userid` char(64) NOT NULL COMMENT 'ÓÃ»§ID',
  `pushid` char(64) NOT NULL COMMENT 'ÍÆËÍID',
  `lastlogintime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÉÏ´ÎµÇÂ½Ê±¼ä',
  PRIMARY KEY (`userid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÍÆËÍÐÅÏ¢±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_pushmessageinfo`
--

LOCK TABLES `t_pushmessageinfo` WRITE;
/*!40000 ALTER TABLE `t_pushmessageinfo` DISABLE KEYS */;
INSERT INTO `t_pushmessageinfo` VALUES ('QMQJ367640','','2015-06-03 00:00:00'),('BD388904727','','2015-06-03 00:00:00'),('LESHI110627628','','2015-06-03 00:00:00'),('XYMU945290','','2015-06-03 00:00:00'),('BD304300864','','2015-06-03 00:00:00');
/*!40000 ALTER TABLE `t_pushmessageinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_qianggoubuy`
--

DROP TABLE IF EXISTS `t_qianggoubuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_qianggoubuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·Ñ',
  `leftmoney` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàÔª±¦',
  `qianggouid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÇÀ¹ºid,t_qianggouitemÖÐµÄId×Ö¶Î£¬È«¾ÖÎ¨Ò»',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  `actstartday` int(11) NOT NULL DEFAULT '0' COMMENT '±ê¼Ç»î¶¯µÄÆðÊ¼ÈÕÆÚ',
  PRIMARY KEY (`Id`),
  KEY `goodsid_qianggouid` (`goodsid`,`qianggouid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÏÞÊ±ÇÀ¹º¹ºÂò¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_qianggoubuy`
--

LOCK TABLES `t_qianggoubuy` WRITE;
/*!40000 ALTER TABLE `t_qianggoubuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_qianggoubuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_qianggouitem`
--

DROP TABLE IF EXISTS `t_qianggouitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_qianggouitem` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `itemgroup` int(11) NOT NULL DEFAULT '0' COMMENT '·Ö×éID',
  `random` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñËæ»ú£¬0²»Ëæ»ú£¬1Ëæ»ú',
  `itemid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÇÀ¹ºÏîid£¬¶ÔÓ¦ÅäÖÃÎÄ¼þÖÐµÄID×Ö¶Î',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `origprice` int(11) NOT NULL DEFAULT '0' COMMENT 'Ô­¼Û',
  `price` int(11) NOT NULL DEFAULT '0' COMMENT 'ÇÀ¹º¼Û',
  `singlepurchase` int(11) NOT NULL DEFAULT '0' COMMENT 'µ¥ÈËÏÞÖÆ¹º',
  `fullpurchase` int(11) NOT NULL DEFAULT '0' COMMENT 'ËùÓÐÈËÏÞ¹º×ÜÁ¿',
  `daystime` int(11) NOT NULL DEFAULT '0' COMMENT 'ÌìÊý',
  `starttime` datetime NOT NULL COMMENT '¿ªÊ¼Ê±¼ä',
  `endtime` datetime NOT NULL COMMENT '½áÊøÊ±¼ä',
  `istimeover` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñ½áÊø',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÏÞÊ±ÇÀ¹ºÇÀ¹ºÏî';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_qianggouitem`
--

LOCK TABLES `t_qianggouitem` WRITE;
/*!40000 ALTER TABLE `t_qianggouitem` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_qianggouitem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_qizhengebuy`
--

DROP TABLE IF EXISTS `t_qizhengebuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_qizhengebuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·Ñ',
  `leftmoney` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàÔª±¦',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÆæÕä¸ó¹ºÂò¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_qizhengebuy`
--

LOCK TABLES `t_qizhengebuy` WRITE;
/*!40000 ALTER TABLE `t_qizhengebuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_qizhengebuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_refreshqizhen`
--

DROP TABLE IF EXISTS `t_refreshqizhen`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_refreshqizhen` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Á÷Ë®±íID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `oldusermoney` int(11) NOT NULL DEFAULT '0' COMMENT 'Ô­ÓÐÔª±¦',
  `leftusermoney` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàÔª±¦',
  `refreshtime` datetime NOT NULL COMMENT 'Ë¢ÐÂÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÆæÕä¸óË¢ÐÂ¼ÍÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_refreshqizhen`
--

LOCK TABLES `t_refreshqizhen` WRITE;
/*!40000 ALTER TABLE `t_refreshqizhen` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_refreshqizhen` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_resourcegetinfo`
--

DROP TABLE IF EXISTS `t_resourcegetinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_resourcegetinfo` (
  `roleid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `type` int(11) NOT NULL DEFAULT '0' COMMENT '»î¶¯ÀàÐÍ',
  `leftcount` int(11) NOT NULL DEFAULT '0' COMMENT 'Î´Íê³É´ÎÊý',
  `exp` int(11) NOT NULL DEFAULT '0' COMMENT '¾­Ñé',
  `bandmoney` int(11) NOT NULL DEFAULT '0' COMMENT '°ó¶¨½ð±Ò',
  `mojing` int(11) NOT NULL DEFAULT '0' COMMENT 'Ä§¾§',
  `chengjiu` int(11) NOT NULL DEFAULT '0' COMMENT '³É¾Í',
  `shengwang` int(11) NOT NULL DEFAULT '0' COMMENT 'ÉùÍû',
  `zhangong` int(11) NOT NULL DEFAULT '0' COMMENT 'Õ½¹¦',
  `bangzuan` int(11) NOT NULL,
  `xinghun` int(11) NOT NULL,
  `hasget` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÕÒ»Ø',
  `yuansufenmo` int(11) NOT NULL DEFAULT '0' COMMENT '×ÊÔ´ÕÒ»ØÔªËØ·ÛÄ©',
  PRIMARY KEY (`roleid`,`type`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='½ÇÉ«×òÈÕ×ÊÔ´ÕÒ»ØÐÅÏ¢';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_resourcegetinfo`
--

LOCK TABLES `t_resourcegetinfo` WRITE;
/*!40000 ALTER TABLE `t_resourcegetinfo` DISABLE KEYS */;
INSERT INTO `t_resourcegetinfo` VALUES (254000000,1,0,0,0,0,0,0,0,0,0,1,0),(254000000,2,0,0,0,0,0,0,0,0,0,1,0),(254000000,3,0,0,0,0,0,0,0,0,0,1,0),(254000000,4,0,0,0,0,0,0,0,0,0,1,0),(254000000,5,0,0,0,0,0,0,0,0,0,1,0),(254000000,6,0,0,0,0,0,0,0,0,0,1,0),(254000000,7,0,0,0,0,0,0,0,0,0,1,0),(254000000,8,0,0,0,0,0,0,0,0,0,1,0),(254000000,9,0,0,0,0,0,0,0,0,0,1,0),(254000000,10,0,0,0,0,0,0,0,0,0,1,0),(254000000,11,0,0,0,0,0,0,0,0,0,1,0),(254000000,12,0,0,0,0,0,0,0,0,0,1,0),(254000000,13,0,0,0,0,0,0,0,0,0,1,0),(254000000,14,0,0,0,0,0,0,0,0,0,1,0),(254000000,15,0,0,0,0,0,0,0,0,0,1,0),(254000000,16,0,0,0,0,0,0,0,0,0,1,0),(254000000,17,0,0,0,0,0,0,0,0,0,1,0),(254000000,18,0,0,0,0,0,0,0,0,0,1,0),(254000000,19,0,0,0,0,0,0,0,0,0,1,0),(254000001,1,0,0,0,0,0,0,0,0,0,1,0),(254000001,2,0,0,0,0,0,0,0,0,0,1,0),(254000001,3,0,0,0,0,0,0,0,0,0,1,0),(254000001,4,0,0,0,0,0,0,0,0,0,1,0),(254000001,5,0,0,0,0,0,0,0,0,0,1,0),(254000001,6,0,0,0,0,0,0,0,0,0,1,0),(254000001,7,0,0,0,0,0,0,0,0,0,1,0),(254000001,8,0,0,0,0,0,0,0,0,0,1,0),(254000001,9,0,0,0,0,0,0,0,0,0,1,0),(254000001,10,0,0,0,0,0,0,0,0,0,1,0),(254000001,11,0,0,0,0,0,0,0,0,0,1,0),(254000001,12,0,0,0,0,0,0,0,0,0,1,0),(254000001,13,0,0,0,0,0,0,0,0,0,1,0),(254000001,14,0,0,0,0,0,0,0,0,0,1,0),(254000001,15,0,0,0,0,0,0,0,0,0,1,0),(254000001,16,0,0,0,0,0,0,0,0,0,1,0),(254000001,17,0,0,0,0,0,0,0,0,0,1,0),(254000001,18,0,0,0,0,0,0,0,0,0,1,0),(254000001,19,0,0,0,0,0,0,0,0,0,1,0),(254000002,1,0,0,0,0,0,0,0,0,0,1,0),(254000002,2,0,0,0,0,0,0,0,0,0,1,0),(254000002,3,0,0,0,0,0,0,0,0,0,1,0),(254000002,4,0,0,0,0,0,0,0,0,0,1,0),(254000002,5,0,0,0,0,0,0,0,0,0,1,0),(254000002,6,0,0,0,0,0,0,0,0,0,1,0),(254000002,7,0,0,0,0,0,0,0,0,0,1,0),(254000002,8,0,0,0,0,0,0,0,0,0,1,0),(254000002,9,0,0,0,0,0,0,0,0,0,1,0),(254000002,10,0,0,0,0,0,0,0,0,0,1,0),(254000002,11,0,0,0,0,0,0,0,0,0,1,0),(254000002,12,0,0,0,0,0,0,0,0,0,1,0),(254000002,13,0,0,0,0,0,0,0,0,0,1,0),(254000002,14,0,0,0,0,0,0,0,0,0,1,0),(254000002,15,0,0,0,0,0,0,0,0,0,1,0),(254000002,16,0,0,0,0,0,0,0,0,0,1,0),(254000002,17,0,0,0,0,0,0,0,0,0,1,0),(254000002,18,0,0,0,0,0,0,0,0,0,1,0),(254000002,19,0,0,0,0,0,0,0,0,0,1,0),(254000003,1,0,0,0,0,0,0,0,0,0,1,0),(254000003,2,0,0,0,0,0,0,0,0,0,1,0),(254000003,3,0,0,0,0,0,0,0,0,0,1,0),(254000003,4,0,0,0,0,0,0,0,0,0,1,0),(254000003,5,0,0,0,0,0,0,0,0,0,1,0),(254000003,6,0,0,0,0,0,0,0,0,0,1,0),(254000003,7,0,0,0,0,0,0,0,0,0,1,0),(254000003,8,0,0,0,0,0,0,0,0,0,1,0),(254000003,9,0,0,0,0,0,0,0,0,0,1,0),(254000003,10,0,0,0,0,0,0,0,0,0,1,0),(254000003,11,0,0,0,0,0,0,0,0,0,1,0),(254000003,12,0,0,0,0,0,0,0,0,0,1,0),(254000003,13,0,0,0,0,0,0,0,0,0,1,0),(254000003,14,0,0,0,0,0,0,0,0,0,1,0),(254000003,15,0,0,0,0,0,0,0,0,0,1,0),(254000003,16,0,0,0,0,0,0,0,0,0,1,0),(254000003,17,0,0,0,0,0,0,0,0,0,1,0),(254000003,18,0,0,0,0,0,0,0,0,0,1,0),(254000003,19,0,0,0,0,0,0,0,0,0,1,0),(254000004,1,0,0,0,0,0,0,0,0,0,1,0),(254000004,2,0,0,0,0,0,0,0,0,0,1,0),(254000004,3,0,0,0,0,0,0,0,0,0,1,0),(254000004,4,0,0,0,0,0,0,0,0,0,1,0),(254000004,5,0,0,0,0,0,0,0,0,0,1,0),(254000004,6,0,0,0,0,0,0,0,0,0,1,0),(254000004,7,0,0,0,0,0,0,0,0,0,1,0),(254000004,8,0,0,0,0,0,0,0,0,0,1,0),(254000004,9,0,0,0,0,0,0,0,0,0,1,0),(254000004,10,0,0,0,0,0,0,0,0,0,1,0),(254000004,11,0,0,0,0,0,0,0,0,0,1,0),(254000004,12,0,0,0,0,0,0,0,0,0,1,0),(254000004,13,0,0,0,0,0,0,0,0,0,1,0),(254000004,14,0,0,0,0,0,0,0,0,0,1,0),(254000004,15,0,0,0,0,0,0,0,0,0,1,0),(254000004,16,0,0,0,0,0,0,0,0,0,1,0),(254000004,17,0,0,0,0,0,0,0,0,0,1,0),(254000004,18,0,0,0,0,0,0,0,0,0,1,0),(254000004,19,0,0,0,0,0,0,0,0,0,1,0),(254000005,1,0,0,0,0,0,0,0,0,0,1,0),(254000005,2,0,0,0,0,0,0,0,0,0,1,0),(254000005,3,0,0,0,0,0,0,0,0,0,1,0),(254000005,4,0,0,0,0,0,0,0,0,0,1,0),(254000005,5,0,0,0,0,0,0,0,0,0,1,0),(254000005,6,0,0,0,0,0,0,0,0,0,1,0),(254000005,7,0,0,0,0,0,0,0,0,0,1,0),(254000005,8,0,0,0,0,0,0,0,0,0,1,0),(254000005,9,0,0,0,0,0,0,0,0,0,1,0),(254000005,10,0,0,0,0,0,0,0,0,0,1,0),(254000005,11,0,0,0,0,0,0,0,0,0,1,0),(254000005,12,0,0,0,0,0,0,0,0,0,1,0),(254000005,13,0,0,0,0,0,0,0,0,0,1,0),(254000005,14,0,0,0,0,0,0,0,0,0,1,0),(254000005,15,0,0,0,0,0,0,0,0,0,1,0),(254000005,16,0,0,0,0,0,0,0,0,0,1,0),(254000005,17,0,0,0,0,0,0,0,0,0,1,0),(254000005,18,0,0,0,0,0,0,0,0,0,1,0),(254000005,19,0,0,0,0,0,0,0,0,0,1,0),(254000006,1,0,0,0,0,0,0,0,0,0,1,0),(254000006,2,0,0,0,0,0,0,0,0,0,1,0),(254000006,3,0,0,0,0,0,0,0,0,0,1,0),(254000006,4,0,0,0,0,0,0,0,0,0,1,0),(254000006,5,0,0,0,0,0,0,0,0,0,1,0),(254000006,6,0,0,0,0,0,0,0,0,0,1,0),(254000006,7,0,0,0,0,0,0,0,0,0,1,0),(254000006,8,0,0,0,0,0,0,0,0,0,1,0),(254000006,9,0,0,0,0,0,0,0,0,0,1,0),(254000006,10,0,0,0,0,0,0,0,0,0,1,0),(254000006,11,0,0,0,0,0,0,0,0,0,1,0),(254000006,12,0,0,0,0,0,0,0,0,0,1,0),(254000006,13,0,0,0,0,0,0,0,0,0,1,0),(254000006,14,0,0,0,0,0,0,0,0,0,1,0),(254000006,15,0,0,0,0,0,0,0,0,0,1,0),(254000006,16,0,0,0,0,0,0,0,0,0,1,0),(254000006,17,0,0,0,0,0,0,0,0,0,1,0),(254000006,18,0,0,0,0,0,0,0,0,0,1,0),(254000006,19,0,0,0,0,0,0,0,0,0,1,0);
/*!40000 ALTER TABLE `t_resourcegetinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_roleparams`
--

DROP TABLE IF EXISTS `t_roleparams`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_roleparams` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `pname` char(32) NOT NULL,
  `pvalue` char(128) DEFAULT NULL,
  UNIQUE KEY `rid_pname` (`rid`,`pname`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Ö÷½Ç²ÎÊý±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_roleparams`
--

LOCK TABLES `t_roleparams` WRITE;
/*!40000 ALTER TABLE `t_roleparams` DISABLE KEYS */;
INSERT INTO `t_roleparams` VALUES (254000000,'AddProPointForLevelUp','5'),(254000000,'VerifyBuffProp','1'),(254000000,'AdmireDayID','154'),(254000000,'AdmireCount','0'),(254000000,'PKKingAdmireDayID','154'),(254000000,'PKKingAdmireCount','0'),(254000000,'JieriLoginNum','0'),(254000000,'JieriLoginDayID','147'),(254000000,'HysyYTDSuccessDayId','1299'),(254000000,'HysyYTDSuccessCount','0'),(254000000,'ChengJiuData','AAAAAAAAAAABAAAAAQAAAA=='),(254000000,'DailyActiveFlag','AQAAAAAAAAA='),(254000000,'DailyActiveInfo1','BQAAAAAAAAABAAAA'),(254000000,'DailyActiveDayID','154'),(254000000,'CallPetFreeTime','112386431.199875'),(254000000,'GuMuAwardDayID','154'),(254000000,'ChongJiGiftList','0,0,0,0,0'),(254000000,'TotalPropPoint','5'),(254000000,'PropIntelligence','2'),(254000000,'PropDexterity','1'),(254000000,'PropConstitution','2'),(254000000,'PropStrength','0'),(254000000,'DailyActiveAwardFlag','0'),(254000000,'FightGetThings','-1'),(254000000,'OpenGridTick','40'),(254000000,'OpenPortableGridTick','40'),(254000000,'CurHP','316'),(254000000,'CurMP','500'),(254000000,'DayOnlineSecond','44'),(254000000,'SeriesLoginCount','1'),(254000000,'DefaultSkillLev','1'),(254000000,'DefaultSkillUseNum','0'),(254000000,'MeditateTime','14361219'),(254000000,'NotSafeMeditateTime','0'),(254000001,'AddProPointForLevelUp','70'),(254000001,'VerifyBuffProp','1'),(254000001,'AdmireDayID','154'),(254000001,'AdmireCount','0'),(254000001,'PKKingAdmireDayID','154'),(254000001,'PKKingAdmireCount','0'),(254000001,'JieriLoginNum','0'),(254000001,'JieriLoginDayID','147'),(254000001,'HysyYTDSuccessDayId','1299'),(254000001,'HysyYTDSuccessCount','0'),(254000001,'ChengJiuData','MgAAAAwAAAABAAAAAQAAAA=='),(254000001,'DailyActiveFlag','AQAAAAAAAAA='),(254000001,'DailyActiveInfo1','BQAAAAwAAAABAAAA'),(254000001,'DailyActiveDayID','154'),(254000001,'CallPetFreeTime','112386966.074875'),(254000001,'GuMuAwardDayID','154'),(254000001,'TotalPropPoint','70'),(254000001,'PropIntelligence','35'),(254000001,'PropDexterity','14'),(254000001,'PropConstitution','21'),(254000001,'ChengJiuFlags','AwAAAAAAAAA='),(254000001,'PropStrength','0'),(254000001,'DailyActiveAwardFlag','0'),(254000001,'FightGetThings','1056964623'),(254000001,'OpenGridTick','410'),(254000001,'OpenPortableGridTick','410'),(254000001,'DefaultSkillUseNum','6'),(254000001,'MeditateTime','120000'),(254000001,'CurHP','2602'),(254000001,'CurMP','500'),(254000001,'DayOnlineSecond','416'),(254000001,'SeriesLoginCount','1'),(254000001,'DefaultSkillLev','1'),(254000001,'NotSafeMeditateTime','13454750'),(254000002,'AddProPointForLevelUp','5'),(254000002,'VerifyBuffProp','1'),(254000002,'AdmireDayID','154'),(254000002,'AdmireCount','0'),(254000002,'PKKingAdmireDayID','154'),(254000002,'PKKingAdmireCount','0'),(254000002,'JieriLoginNum','0'),(254000002,'JieriLoginDayID','147'),(254000002,'HysyYTDSuccessDayId','1299'),(254000002,'HysyYTDSuccessCount','0'),(254000002,'ChengJiuData','AAAAAAAAAAABAAAAAQAAAA=='),(254000002,'DailyActiveFlag','AQAAAAAAAAA='),(254000002,'DailyActiveInfo1','BQAAAAAAAAABAAAA'),(254000002,'DailyActiveDayID','154'),(254000002,'CallPetFreeTime','112391853.824875'),(254000002,'GuMuAwardDayID','154'),(254000002,'TotalPropPoint','5'),(254000002,'PropStrength','2'),(254000002,'PropDexterity','1'),(254000002,'PropConstitution','2'),(254000002,'PropIntelligence','0'),(254000002,'DailyActiveAwardFlag','0'),(254000002,'FightGetThings','-1'),(254000002,'OpenGridTick','440'),(254000002,'OpenPortableGridTick','440'),(254000002,'CurHP','518'),(254000002,'CurMP','500'),(254000002,'DayOnlineSecond','450'),(254000002,'SeriesLoginCount','1'),(254000002,'DefaultSkillLev','1'),(254000002,'DefaultSkillUseNum','0'),(254000002,'MeditateTime','8768594'),(254000002,'NotSafeMeditateTime','0'),(254000003,'AddProPointForLevelUp','5'),(254000003,'VerifyBuffProp','1'),(254000003,'AdmireDayID','154'),(254000003,'AdmireCount','0'),(254000003,'PKKingAdmireDayID','154'),(254000003,'PKKingAdmireCount','0'),(254000003,'JieriLoginNum','0'),(254000003,'JieriLoginDayID','147'),(254000003,'HysyYTDSuccessDayId','1299'),(254000003,'HysyYTDSuccessCount','0'),(254000003,'ChengJiuData','AAAAAAAAAAABAAAAAQAAAA=='),(254000003,'DailyActiveFlag','AQAAAAAAAAA='),(254000003,'DailyActiveInfo1','BQAAAAAAAAABAAAA'),(254000003,'DailyActiveDayID','154'),(254000003,'CallPetFreeTime','112401054.324875'),(254000003,'GuMuAwardDayID','154'),(254000003,'TotalPropPoint','5'),(254000003,'PropStrength','2'),(254000003,'PropDexterity','1'),(254000003,'PropConstitution','2'),(254000004,'AddProPointForLevelUp','5'),(254000004,'VerifyBuffProp','1'),(254000004,'AdmireDayID','154'),(254000004,'AdmireCount','0'),(254000004,'PKKingAdmireDayID','154'),(254000004,'PKKingAdmireCount','0'),(254000004,'JieriLoginNum','0'),(254000004,'JieriLoginDayID','147'),(254000004,'HysyYTDSuccessDayId','1299'),(254000004,'HysyYTDSuccessCount','0'),(254000004,'ChengJiuData','AAAAAAAAAAABAAAAAQAAAA=='),(254000004,'DailyActiveFlag','AQAAAAAAAAA='),(254000004,'DailyActiveInfo1','BQAAAAAAAAABAAAA'),(254000004,'DailyActiveDayID','154'),(254000004,'CallPetFreeTime','112401071.637375'),(254000003,'PropIntelligence','0'),(254000003,'DailyActiveAwardFlag','0'),(254000003,'FightGetThings','-1'),(254000003,'OpenGridTick','40'),(254000003,'OpenPortableGridTick','40'),(254000003,'CurHP','368'),(254000003,'CurMP','500'),(254000003,'DayOnlineSecond','58'),(254000003,'SeriesLoginCount','1'),(254000003,'DefaultSkillLev','1'),(254000003,'DefaultSkillUseNum','0'),(254000003,'MeditateTime','0'),(254000003,'NotSafeMeditateTime','0'),(254000004,'GuMuAwardDayID','154'),(254000004,'PropStrength','0'),(254000004,'PropIntelligence','2'),(254000004,'PropDexterity','1'),(254000004,'PropConstitution','2'),(254000004,'DailyActiveAwardFlag','0'),(254000004,'FightGetThings','-1'),(254000004,'OpenGridTick','40'),(254000004,'OpenPortableGridTick','40'),(254000004,'CurHP','316'),(254000004,'CurMP','500'),(254000004,'TotalPropPoint','5'),(254000004,'DayOnlineSecond','78'),(254000004,'SeriesLoginCount','1'),(254000004,'DefaultSkillLev','1'),(254000004,'DefaultSkillUseNum','0'),(254000004,'MeditateTime','0'),(254000004,'NotSafeMeditateTime','0'),(254000003,'TotalLoginAwardFlag','2'),(254000005,'AddProPointForLevelUp','10'),(254000005,'VerifyBuffProp','1'),(254000005,'AdmireDayID','154'),(254000005,'AdmireCount','0'),(254000005,'PKKingAdmireDayID','154'),(254000005,'PKKingAdmireCount','0'),(254000005,'JieriLoginNum','0'),(254000005,'JieriLoginDayID','147'),(254000005,'HysyYTDSuccessDayId','1299'),(254000005,'HysyYTDSuccessCount','0'),(254000005,'ChengJiuData','AAAAAAAAAAABAAAAAQAAAA=='),(254000005,'DailyActiveFlag','AQAAAAAAAAA='),(254000005,'DailyActiveInfo1','BQAAAAAAAAABAAAA'),(254000005,'DailyActiveDayID','154'),(254000005,'CallPetFreeTime','112401134.12175'),(254000005,'GuMuAwardDayID','154'),(254000006,'AddProPointForLevelUp','0'),(254000006,'VerifyBuffProp','1'),(254000006,'AdmireDayID','154'),(254000006,'AdmireCount','0'),(254000006,'PKKingAdmireDayID','154'),(254000006,'PKKingAdmireCount','0'),(254000006,'JieriLoginNum','0'),(254000006,'JieriLoginDayID','147'),(254000006,'HysyYTDSuccessDayId','1299'),(254000006,'HysyYTDSuccessCount','0'),(254000006,'ChengJiuData','AAAAAAAAAAABAAAAAQAAAA=='),(254000006,'DailyActiveFlag','AQAAAAAAAAA='),(254000006,'DailyActiveInfo1','BQAAAAAAAAABAAAA'),(254000006,'DailyActiveDayID','154'),(254000006,'CallPetFreeTime','112401138.231125'),(254000005,'TotalPropPoint','10'),(254000005,'PropStrength','5'),(254000005,'PropDexterity','2'),(254000005,'PropConstitution','3'),(254000006,'GuMuAwardDayID','154'),(254000006,'PropStrength','0'),(254000006,'PropIntelligence','0'),(254000006,'PropDexterity','0'),(254000006,'PropConstitution','0'),(254000006,'DailyActiveAwardFlag','0'),(254000006,'FightGetThings','-1'),(254000006,'OpenGridTick','10'),(254000006,'OpenPortableGridTick','10'),(254000006,'CurHP','360'),(254000006,'CurMP','500'),(254000006,'TotalPropPoint','0'),(254000006,'DayOnlineSecond','12'),(254000006,'SeriesLoginCount','1'),(254000006,'DefaultSkillLev','1'),(254000006,'DefaultSkillUseNum','0'),(254000006,'MeditateTime','0'),(254000006,'NotSafeMeditateTime','0'),(254000005,'PropIntelligence','0'),(254000005,'DailyActiveAwardFlag','0'),(254000005,'FightGetThings','-1'),(254000005,'OpenGridTick','40'),(254000005,'OpenPortableGridTick','40'),(254000005,'CurHP','444'),(254000005,'CurMP','500'),(254000005,'DayOnlineSecond','45'),(254000005,'SeriesLoginCount','1'),(254000005,'DefaultSkillLev','1'),(254000005,'DefaultSkillUseNum','0'),(254000005,'MeditateTime','0'),(254000005,'NotSafeMeditateTime','0');
/*!40000 ALTER TABLE `t_roleparams` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_roles`
--

DROP TABLE IF EXISTS `t_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_roles` (
  `rid` int(11) NOT NULL AUTO_INCREMENT COMMENT '½ÇÉ«ID',
  `userid` char(64) NOT NULL COMMENT 'ÓÃ»§ID',
  `rname` char(32) DEFAULT NULL,
  `sex` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÐÔ±ð',
  `occupation` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'Ö°Òµ',
  `level` smallint(6) unsigned NOT NULL DEFAULT '1' COMMENT '¼¶±ð',
  `pic` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'Í·Ïñ',
  `faction` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '°ïÅÉ',
  `money1` int(11) NOT NULL DEFAULT '0' COMMENT '°ó¶¨½ð±Ò',
  `money2` int(11) NOT NULL DEFAULT '0' COMMENT '°ó¶¨ÓÎÏ·±Ò(±¸ÓÃ)',
  `experience` bigint(20) unsigned NOT NULL DEFAULT '0' COMMENT 'µ±Ç°µÄ¾­ÑéÖµ',
  `pkmode` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'µ±Ç°µÄPKÄ£Ê½',
  `pkvalue` int(11) NOT NULL DEFAULT '0' COMMENT 'PKÖµ',
  `position` char(32) NOT NULL DEFAULT '-1:0:-1:-1' COMMENT 'ËùÔÚµØÍ¼£¬Î»ÖÃX, Î»ÖÃY, ·½Ïò',
  `regtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '×¢²áÊ±¼ä',
  `lasttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '×îºóµÇÂ¼Ê±¼ä',
  `isdel` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÒÑ¾­É¾³ý',
  `deltime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'É¾³ýÊ±¼ä',
  `bagnum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '±³°üµÄ¸ñ×ÓµÄ¸öÊý(Ä¬ÈÏÊÇ42)',
  `othername` char(32) NOT NULL DEFAULT '' COMMENT '³ÆºÅ',
  `main_quick_keys` char(255) NOT NULL DEFAULT '' COMMENT 'Ö÷¿ì½Ý¼üÓ³Éä',
  `other_quick_keys` char(255) NOT NULL DEFAULT '' COMMENT '¸¨Öú¿ì½Ý¼üÓ³Éä',
  `loginnum` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'µÇÂ¼´ÎÊý',
  `leftfightsecs` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàµÄ×Ô¶¯¹Ò»úÊ±¼ä(µ¥Î»Ãë)',
  `horseid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¶ÔÓ¦×øÆï±íÖÐµÄÊý¾Ý¿âID',
  `petid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¶ÔÓ¦³èÎï±íÖÐµÄÊý¾Ý¿âid',
  `interpower` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '½ÇÉ«µÄÄÚÁ¦Öµ',
  `totalonlinesecs` int(11) NOT NULL DEFAULT '0' COMMENT '×ÜµÄÔÚÏßÃëÊý',
  `antiaddictionsecs` int(11) NOT NULL DEFAULT '0' COMMENT '·À³ÁÃÔÔÚÏßµÄÃëÊý',
  `logofftime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT 'ÉÏ´ÎÀëÏßÊ±¼ä',
  `biguantime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT ' ±¾´Î±Õ¹ØµÄ¿ªÊ¼Ê±¼ä',
  `yinliang` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '½ð±Ò',
  `total_jingmai_exp` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '´Ó±ðÈË³åÂö»ñÈ¡µÄ¾­ÑéÖµ(ÀÛ¼Ó)',
  `jingmai_exp_num` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '´Ó±ðÈË³åÂö»ñÈ¡µÄ¾­ÑéµÄ´ÎÊý',
  `lasthorseid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÉÏÒ»´ÎµÄ×øÆïµÄID',
  `skillid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'È±Ê¡µÄ¼¼ÄÜID',
  `autolife` int(11) NOT NULL DEFAULT '70' COMMENT '×Ô¶¯²¹ÑªºÈÒ©µÄ°Ù·Ö±È',
  `automagic` int(11) NOT NULL DEFAULT '50' COMMENT '×Ô¶¯²¹À¶ºÈÒ©µÄ°Ù·Ö±È',
  `numskillid` int(11) NOT NULL DEFAULT '0' COMMENT 'Ôö¼ÓÊìÁ·¶ÈµÄ±»¶¯¼¼ÄÜ',
  `maintaskid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­Íê³ÉµÄÖ÷ÏßÈÎÎñID',
  `pkpoint` int(11) NOT NULL DEFAULT '0' COMMENT 'PK µã',
  `lianzhan` int(11) NOT NULL DEFAULT '0' COMMENT 'Á¬Õ¶Êý',
  `killboss` int(11) NOT NULL DEFAULT '0' COMMENT 'É±BOSSµÄ¸öÊý',
  `equipjifen` int(11) NOT NULL DEFAULT '0' COMMENT 'ËùÓÐ×°±¸µÄ»ý·Ö',
  `xueweinum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­³åÍ¨µÄÑ¨Î»µÄ¸öÊý',
  `skilllearnednum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­Éý¼¶µÄ¼¼ÄÜ²ãÊý',
  `horsejifen` int(11) NOT NULL DEFAULT '0' COMMENT '×øÆïµÄ»ý·Ö',
  `battlenamestart` bigint(20) NOT NULL DEFAULT '0' COMMENT '½Ç¶·³¡ÈÙÓþ³ÆºÅ¿ªÊ¼Ê±¼ä',
  `battlenameindex` int(11) NOT NULL DEFAULT '0' COMMENT '½Ç¶·³¡ÈÙÓþ³ÆºÅ',
  `cztaskid` int(11) NOT NULL DEFAULT '0' COMMENT '³äÖµTaskID',
  `battlenum` int(11) NOT NULL DEFAULT '0' COMMENT '½Ç¶·³¡³ÆºÅ´ÎÊý',
  `heroindex` int(11) NOT NULL DEFAULT '0' COMMENT 'Ó¢ÐÛÖðÀÞµÄ×î¸ß²ãÊý',
  `logindayid` int(11) NOT NULL DEFAULT '0' COMMENT 'µÇÂ¼ÈÕID',
  `logindaynum` int(11) NOT NULL DEFAULT '0' COMMENT 'µÇÂ¼ÈÕ´ÎÊý',
  `zoneid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÇøºÅ',
  `bhname` char(32) NOT NULL COMMENT '°ï»áÃû³Æ',
  `bhverify` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '±»ÑûÇë¼ÓÈë°ï»áÊ±ÊÇ·ñÑéÖ¤',
  `bhzhiwu` int(11) NOT NULL DEFAULT '0' COMMENT '°ïÖÐÖ°Îñ(0: ÆÕÍ¨°ïÖú, 1: °ïÖ÷, 2: ¸±°ïÖ÷, 3: ×ó»¤·¨, 4: ÓÒ»¤·¨)',
  `chenghao` char(32) NOT NULL COMMENT '°ïÖÐ³ÆºÅ',
  `bgdayid1` int(11) NOT NULL DEFAULT '0' COMMENT '°ï¹±ÈÕID1',
  `bgmoney` int(11) NOT NULL DEFAULT '0' COMMENT 'Ã¿ÈÕÍ­Ç®°ï¹±',
  `bgdayid2` int(11) NOT NULL DEFAULT '0' COMMENT '°ï¹±ÈÕID2',
  `bggoods` int(11) NOT NULL DEFAULT '0' COMMENT 'Ã¿ÈÕµÀ¾ß°ï¹±',
  `banggong` int(11) NOT NULL DEFAULT '0' COMMENT '°ï¹±',
  `huanghou` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñ»Êºó',
  `jiebiaodayid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÙïÚµÄÈÕID',
  `jiebiaonum` int(11) NOT NULL DEFAULT '0' COMMENT 'Ã¿ÈÕ½ÙïÚµÄ´ÎÊý',
  `username` char(64) NOT NULL COMMENT 'Æ½Ì¨µÄÓÃ»§Ãû³Æ',
  `lastmailid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×îÐÂÓÊ¼þµÄÓÊ¼þID',
  `onceawardflag` bigint(11) unsigned NOT NULL DEFAULT '0' COMMENT 'µ¥´ÎÈÎÎñ½±Àø¼ÇÂ¼×Ö¶Î',
  `banchat` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÓÀ¾Ã½ûÑÔ',
  `banlogin` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÓÀ¾Ã½ûÖ¹µÇÂ½',
  `isflashplayer` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÎªÐÂÊÖÓÃ»§1ÎªÐÂÊÖ£¬0ÎªÕý³£',
  `changelifecount` int(11) NOT NULL DEFAULT '0' COMMENT '×ªÉúµÈ¼¶',
  `admiredcount` int(11) NOT NULL DEFAULT '0' COMMENT '±»³ç°Ý¼ÆÊý',
  `combatforce` int(11) NOT NULL DEFAULT '0' COMMENT 'Õ½¶·Á¦',
  `autoassignpropertypoint` int(11) NOT NULL DEFAULT '1' COMMENT '×Ô¶¯·ÖÅäµãÊý',
  `vipawardflag` int(11) NOT NULL DEFAULT '0' COMMENT 'VIP½±ÀøÁìÈ¡±ê¼Ç',
  `store_yinliang` bigint(20) unsigned NOT NULL DEFAULT '0' COMMENT '²Ö¿â½ð±Ò',
  `store_money` bigint(20) unsigned NOT NULL DEFAULT '0' COMMENT '²Ö¿â°ó¶¨½ð±Ò',
  UNIQUE KEY `rid` (`rid`),
  UNIQUE KEY `rname_zoneid` (`rname`,`zoneid`),
  KEY `userid` (`userid`)
) ENGINE=MyISAM AUTO_INCREMENT=254000007 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_roles`
--

LOCK TABLES `t_roles` WRITE;
/*!40000 ALTER TABLE `t_roles` DISABLE KEYS */;
INSERT INTO `t_roles` VALUES (254000000,'QMQJ367640','Âí¿­Î÷¡¤Ê©ÄÍµÂ',0,1,2,0,0,1972,0,0,0,0,'1:7:11150:11950','2015-06-03 18:27:10','2015-06-03 18:27:14',0,'1900-01-01 12:00:00',50,'','','',1,0,0,0,0,40,40,'2015-06-03 18:27:55','2015-06-03 18:27:10',0,0,0,0,0,70,50,0,1000,0,0,0,0,0,0,0,0,0,0,0,0,154,1,1270,'',0,0,'',0,0,0,0,0,0,0,0,'QMQJ367640',0,0,0,0,0,0,0,445,1,0,0,0),(254000001,'BD388904727','Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',0,1,15,0,0,79870,20,470,0,0,'1:1:11850:16350','2015-06-03 18:36:05','2015-06-03 18:36:12',0,'1900-01-01 12:00:00',50,'','-1@0|1@200|0@204|-1@0','',1,0,0,0,0,410,410,'2015-06-03 18:43:01','2015-06-03 18:36:05',0,0,0,0,0,70,50,0,1100,0,0,0,0,0,0,0,0,0,0,0,0,154,1,1270,'',0,0,'',0,0,0,0,0,0,0,0,'BD388904727',0,0,0,0,0,0,0,3819,1,0,0,0),(254000002,'LESHI110627628','ÄÝæ«¡¤Â³',1,2,2,0,0,1972,100,0,0,0,'1:3:10350:11550','2015-06-03 19:57:33','2015-06-03 19:58:26',0,'1900-01-01 12:00:00',50,'','','',2,0,0,0,0,440,440,'2015-06-03 20:05:07','2015-06-03 19:57:33',0,0,0,0,0,70,50,0,1000,0,0,0,0,0,0,0,0,0,0,0,0,154,1,1270,'',0,0,'',0,0,0,0,0,0,0,0,'LESHI110627628',0,0,0,0,0,0,0,749,1,0,0,0),(254000003,'XYMU945290','AD1270',1,2,2,0,0,1972,0,0,0,0,'1:3:11150:10850','2015-06-03 22:30:53','2015-06-03 22:32:05',0,'1900-01-01 12:00:00',50,'','','',4,0,0,0,0,40,40,'2015-06-03 22:32:10','2015-06-03 22:30:53',0,0,0,0,0,70,50,0,1000,0,0,0,0,0,0,0,0,0,0,0,0,154,1,1270,'',0,0,'',0,0,0,0,0,0,0,0,'XYMU945290',0,0,0,0,0,0,0,443,1,0,0,0),(254000004,'BD304300864','Ë¹¿¨À¼¡¤ÃÀÀû',0,1,2,0,0,1972,0,0,0,0,'1:7:11050:12050','2015-06-03 22:31:11','2015-06-03 22:32:44',0,'1900-01-01 12:00:00',50,'','','',6,0,0,0,0,40,40,'2015-06-03 22:33:02','2015-06-03 22:31:11',0,0,0,0,0,70,50,0,1000,0,0,0,0,0,0,0,0,0,0,0,0,154,1,1270,'',0,0,'',0,0,0,0,0,0,0,0,'BD304300864',0,0,0,0,0,0,0,445,1,0,0,0),(254000005,'XYMU945290','Ë¹ÌØÀ­¡¤ÂåÅå',0,0,3,0,0,4240,0,10,0,0,'1:1:11350:10650','2015-06-03 22:32:14','2015-06-03 22:32:15',0,'1900-01-01 12:00:00',50,'','','',1,0,0,0,0,40,40,'2015-06-03 22:32:58','2015-06-03 22:32:14',0,0,0,0,0,70,50,0,1010,0,0,0,0,0,0,0,0,0,0,0,0,154,1,1270,'',0,0,'',0,0,0,0,0,0,0,0,'XYMU945290',0,0,0,0,0,0,0,469,1,0,0,0),(254000006,'BD304300864','¼§Âê¡¤ÂõÒ®',1,2,1,0,0,0,0,0,0,0,'1:3:11450:10750','2015-06-03 22:32:18','2015-06-03 22:32:22',0,'1900-01-01 12:00:00',50,'','','',1,0,0,0,0,10,10,'2015-06-03 22:32:30','2015-06-03 22:32:18',0,0,0,0,0,70,50,0,0,0,0,0,0,0,0,0,0,0,0,0,0,154,1,1270,'',0,0,'',0,0,0,0,0,0,0,0,'BD304300864',0,0,0,0,0,0,0,436,1,0,0,0);
/*!40000 ALTER TABLE `t_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_secondpassword`
--

DROP TABLE IF EXISTS `t_secondpassword`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_secondpassword` (
  `userid` varchar(64) NOT NULL COMMENT 'ÕËºÅ',
  `secpwd` varchar(32) NOT NULL DEFAULT '' COMMENT '¼ÓÃÜºóµÄ¶þ¼¶ÃÜÂë',
  PRIMARY KEY (`userid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='¶þ¼¶ÃÜÂë';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_secondpassword`
--

LOCK TABLES `t_secondpassword` WRITE;
/*!40000 ALTER TABLE `t_secondpassword` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_secondpassword` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_shengxiaoguesshist`
--

DROP TABLE IF EXISTS `t_shengxiaoguesshist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_shengxiaoguesshist` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `rname` char(32) DEFAULT NULL,
  `zoneid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ÇøºÅ',
  `guesskey` int(11) NOT NULL DEFAULT '0' COMMENT '¾º²Â¹Ø¼ü×Ö',
  `mortgage` int(11) NOT NULL DEFAULT '0' COMMENT '×¢Âë',
  `resultkey` int(11) NOT NULL DEFAULT '0' COMMENT '½á¹û¹Ø¼ü×Ö',
  `gainnum` int(11) NOT NULL DEFAULT '0' COMMENT 'Ó®È¡ÊýÁ¿,0±íÊ¾Êä£¬´óÓÚ0±íÊ¾Ó®',
  `leftmortgage` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£Óà×¢Âë',
  `guesstime` datetime NOT NULL COMMENT '¾º²ÂÊ±¼ä£¬½á¹û³öÀ´µÄÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÉúÐ¤ÔË³Ì¾º²Â¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_shengxiaoguesshist`
--

LOCK TABLES `t_shengxiaoguesshist` WRITE;
/*!40000 ALTER TABLE `t_shengxiaoguesshist` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_shengxiaoguesshist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_skills`
--

DROP TABLE IF EXISTS `t_skills`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_skills` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `skillid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¼¼ÄÜID',
  `skilllevel` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '¼¼ÄÜ¼¶±ð',
  `usednum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊìÁ·¶È',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `rid_skillid` (`rid`,`skillid`),
  KEY `rid` (`rid`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='¼¼ÄÜÉý¼¶±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_skills`
--

LOCK TABLES `t_skills` WRITE;
/*!40000 ALTER TABLE `t_skills` DISABLE KEYS */;
INSERT INTO `t_skills` VALUES (1,254000001,204,1,5);
/*!40000 ALTER TABLE `t_skills` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_starconstellationinfo`
--

DROP TABLE IF EXISTS `t_starconstellationinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_starconstellationinfo` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `roleid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `starsiteid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÐÇ×ùID',
  `starslotid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÐÇÎ»ID',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `roleid_starconstellation` (`roleid`,`starsiteid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÐÇ×ùÐÅÏ¢±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_starconstellationinfo`
--

LOCK TABLES `t_starconstellationinfo` WRITE;
/*!40000 ALTER TABLE `t_starconstellationinfo` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_starconstellationinfo` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_tasks`
--

DROP TABLE IF EXISTS `t_tasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_tasks` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `taskid` int(11) NOT NULL DEFAULT '0',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `focus` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñ×·×ÙÈÎÎñ',
  `value1` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈÎÎñ¼ÆÊý1(´ò¹ÖµÄ¼ÆÊý£¬ÆäËûµÄ²½Öè)',
  `value2` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈÎÎñ¼ÆÊý2(´ò¹ÖµÄ¼ÆÊý£¬ÆäËûµÄ²½Öè)',
  `isdel` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÉ¾³ý',
  `addtime` datetime NOT NULL COMMENT 'ÈÎÎñÌí¼ÓµÄÊ±¼ä',
  `starlevel` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `rid` (`rid`)
) ENGINE=MyISAM AUTO_INCREMENT=28 DEFAULT CHARSET=utf8 COMMENT='ÈÎÎñ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_tasks`
--

LOCK TABLES `t_tasks` WRITE;
/*!40000 ALTER TABLE `t_tasks` DISABLE KEYS */;
INSERT INTO `t_tasks` VALUES (1,1000,254000000,1,1,0,1,'2015-06-03 18:27:10',1),(2,1010,254000000,1,0,0,0,'2015-06-03 18:27:49',1),(3,1000,254000001,1,1,0,1,'2015-06-03 18:36:06',1),(4,1010,254000001,1,1,0,1,'2015-06-03 18:36:24',1),(5,1020,254000001,1,1,0,1,'2015-06-03 18:36:36',1),(6,1030,254000001,1,2,0,1,'2015-06-03 18:36:50',1),(7,1040,254000001,1,2,0,1,'2015-06-03 18:37:05',1),(8,1050,254000001,1,2,0,1,'2015-06-03 18:37:26',1),(9,1051,254000001,1,2,0,1,'2015-06-03 18:37:53',1),(10,1060,254000001,1,1,0,1,'2015-06-03 18:38:15',1),(11,1061,254000001,1,1,0,1,'2015-06-03 18:38:27',1),(12,1070,254000001,1,1,0,1,'2015-06-03 18:38:38',1),(13,1071,254000001,1,1,0,1,'2015-06-03 18:38:45',1),(14,1080,254000001,1,1,0,1,'2015-06-03 18:38:58',1),(15,1090,254000001,1,1,0,1,'2015-06-03 18:39:00',1),(16,1100,254000001,1,3,0,1,'2015-06-03 18:39:33',1),(17,1110,254000001,1,0,0,0,'2015-06-03 18:39:55',1),(18,1000,254000002,1,1,0,1,'2015-06-03 19:57:33',1),(19,1010,254000002,1,0,0,0,'2015-06-03 19:57:57',1),(20,1000,254000003,1,1,0,1,'2015-06-03 22:30:54',1),(21,1010,254000003,1,0,0,0,'2015-06-03 22:31:10',1),(22,1000,254000004,1,1,0,1,'2015-06-03 22:31:11',1),(23,1000,254000005,1,1,0,1,'2015-06-03 22:32:14',1),(24,1000,254000006,1,0,0,0,'2015-06-03 22:32:18',1),(25,1010,254000005,1,1,0,1,'2015-06-03 22:32:22',1),(26,1020,254000005,1,0,0,0,'2015-06-03 22:32:36',1),(27,1010,254000004,1,0,0,0,'2015-06-03 22:32:58',1);
/*!40000 ALTER TABLE `t_tasks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_taskslog`
--

DROP TABLE IF EXISTS `t_taskslog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_taskslog` (
  `rid` int(11) NOT NULL DEFAULT '0',
  `taskid` int(11) NOT NULL DEFAULT '0',
  `count` int(11) unsigned NOT NULL DEFAULT '0',
  UNIQUE KEY `taskid_rid` (`rid`,`taskid`),
  KEY `rid` (`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_taskslog`
--

LOCK TABLES `t_taskslog` WRITE;
/*!40000 ALTER TABLE `t_taskslog` DISABLE KEYS */;
INSERT INTO `t_taskslog` VALUES (254000000,1000,1),(254000001,1000,1),(254000001,1010,1),(254000001,1020,1),(254000001,1030,1),(254000001,1040,1),(254000001,1050,1),(254000001,1051,1),(254000001,1060,1),(254000001,1061,1),(254000001,1070,1),(254000001,1071,1),(254000001,1080,1),(254000001,1090,1),(254000001,1100,1),(254000002,1000,1),(254000003,1000,1),(254000005,1000,1),(254000005,1010,1),(254000004,1000,1);
/*!40000 ALTER TABLE `t_taskslog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_tempmoney`
--

DROP TABLE IF EXISTS `t_tempmoney`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_tempmoney` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uid` char(64) NOT NULL,
  `addmoney` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='³äÖµÍ¨Öª±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_tempmoney`
--

LOCK TABLES `t_tempmoney` WRITE;
/*!40000 ALTER TABLE `t_tempmoney` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_tempmoney` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_usedlipinma`
--

DROP TABLE IF EXISTS `t_usedlipinma`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_usedlipinma` (
  `lipinma` char(32) NOT NULL COMMENT 'ÀñÆ·Âë×Ö·û´®',
  `huodongid` int(11) NOT NULL DEFAULT '0' COMMENT '»î¶¯ID',
  `ptid` int(11) NOT NULL DEFAULT '0' COMMENT 'Æ½Ì¨ID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  KEY `rid` (`rid`),
  KEY `huodongid` (`huodongid`),
  KEY `ptid` (`ptid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÒÑ¾­Ê¹ÓÃµÄÀñÆ·Âë±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_usedlipinma`
--

LOCK TABLES `t_usedlipinma` WRITE;
/*!40000 ALTER TABLE `t_usedlipinma` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_usedlipinma` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_usemoney_log`
--

DROP TABLE IF EXISTS `t_usemoney_log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_usemoney_log` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DBId` int(11) DEFAULT NULL COMMENT '物品在物品表的数据库ID，不是物品时Id为-1',
  `userid` varchar(64) DEFAULT NULL COMMENT '用户的平台ID',
  `ObjName` char(32) DEFAULT NULL COMMENT '操作对象名称',
  `optFrom` char(32) DEFAULT NULL COMMENT '操作产生点',
  `currEnvName` char(32) DEFAULT NULL COMMENT '对象所在当前环境名称',
  `tarEnvName` char(32) DEFAULT NULL COMMENT '对象将要到达的环境名称',
  `optType` char(6) DEFAULT NULL COMMENT '操作类型，如下：增加、销毁、修改、移动',
  `optTime` datetime DEFAULT NULL COMMENT '操作时间',
  `optAmount` int(11) DEFAULT NULL COMMENT '操作数量',
  `zoneID` int(11) DEFAULT NULL COMMENT '区编号',
  `optSurplus` int(11) DEFAULT NULL COMMENT 'ÊôÐÔ²Ù×÷ºóµÄÊ£ÓàÖµ',
  PRIMARY KEY (`Id`),
  KEY `DBId` (`DBId`),
  KEY `tarEnvName` (`tarEnvName`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='物品操作日志表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_usemoney_log`
--

LOCK TABLES `t_usemoney_log` WRITE;
/*!40000 ALTER TABLE `t_usemoney_log` DISABLE KEYS */;
INSERT INTO `t_usemoney_log` VALUES (1,-1,'LESHI110627628','×êÊ¯','GMÃüÁîÇ¿ÆÈ¸üÐÂ','ÏµÍ³','ÄÝæ«¡¤Â³','Ôö¼Ó','2015-06-03 20:03:46',10,1270,100);
/*!40000 ALTER TABLE `t_usemoney_log` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_user_active_info`
--

DROP TABLE IF EXISTS `t_user_active_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_user_active_info` (
  `Account` varchar(64) NOT NULL,
  `createTime` date NOT NULL COMMENT 'ÕÊºÅ´´½¨ÈÕÆÚ',
  `seriesLoginCount` int(11) NOT NULL DEFAULT '0' COMMENT 'Á¬ÐøµÇÂ¼ÌìÊý',
  `lastSeriesLoginTime` date NOT NULL COMMENT '×îºóÁ¬ÐøµÇÂ½ÈÕÆÚ',
  PRIMARY KEY (`Account`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÓÃ»§»îÔ¾Í³¼ÆÐÅÏ¢';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_user_active_info`
--

LOCK TABLES `t_user_active_info` WRITE;
/*!40000 ALTER TABLE `t_user_active_info` DISABLE KEYS */;
INSERT INTO `t_user_active_info` VALUES ('QMQJ367640','2015-06-03',1,'2015-06-03'),('BD388904727','2015-06-03',1,'2015-06-03'),('LESHI110627628','2015-06-03',1,'2015-06-03'),('XYMU945290','2015-06-03',1,'2015-06-03'),('BD304300864','2015-06-03',1,'2015-06-03');
/*!40000 ALTER TABLE `t_user_active_info` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_userstat`
--

DROP TABLE IF EXISTS `t_userstat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_userstat` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `userid` char(64) NOT NULL DEFAULT '0' COMMENT 'Æ½Ì¨ÓÃ»§ID',
  `serverid` int(11) NOT NULL DEFAULT '0',
  `eventid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÂ¼þID',
  `rectime` int(11) NOT NULL DEFAULT '0' COMMENT '¼ÇÂ¼Ê±¼ä',
  `loginnum` int(11) NOT NULL DEFAULT '0' COMMENT 'µÇÂ¼´ÎÊý',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `userid_serverid` (`userid`,`serverid`),
  KEY `eventid` (`eventid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Æ½Ì¨ÓÃ»§Í³¼Æ';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_userstat`
--

LOCK TABLES `t_userstat` WRITE;
/*!40000 ALTER TABLE `t_userstat` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_userstat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_vipdailydata`
--

DROP TABLE IF EXISTS `t_vipdailydata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_vipdailydata` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `prioritytype` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÌØÈ¨ÀàÐÍ',
  `dayid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈÕÆÚID',
  `usedtimes` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÒÑ¾­Ê¹ÓÃµÄ´ÎÊý',
  UNIQUE KEY `rid_prioritytype` (`rid`,`prioritytype`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='VIPÃ¿ÈÕÊý¾Ý';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_vipdailydata`
--

LOCK TABLES `t_vipdailydata` WRITE;
/*!40000 ALTER TABLE `t_vipdailydata` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_vipdailydata` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_wanmota`
--

DROP TABLE IF EXISTS `t_wanmota`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_wanmota` (
  `roleID` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `roleName` char(32) DEFAULT NULL,
  `flushTime` bigint(20) NOT NULL DEFAULT '0' COMMENT 'Í¨¹ØÊ±µÄÊ±¼ä',
  `passLayerCount` int(11) NOT NULL DEFAULT '0' COMMENT 'Í¨¹ý²ãÊý',
  `sweepLayer` int(11) DEFAULT '0' COMMENT 'É¨µ´³É¹¦µÄ²ãÊý',
  `sweepReward` text COMMENT '¸÷²ãÉ¨µ´³É¹¦ºó½±Àø¸÷²ã½±ÀøÊý¾Ý:¾­Ñé¡¢½ðÇ®¡¢ÎïÆ·Êý¾Ý',
  `sweepBeginTime` bigint(20) DEFAULT '0' COMMENT 'É¨µ´¿ªÊ¼Ê±¼ä',
  PRIMARY KEY (`roleID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÍòÄ§ËþÊý¾Ý´æ´¢±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_wanmota`
--

LOCK TABLES `t_wanmota` WRITE;
/*!40000 ALTER TABLE `t_wanmota` DISABLE KEYS */;
INSERT INTO `t_wanmota` VALUES (254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',63568952830762,0,-1,'',0),(254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',63568953366043,0,-1,'',0),(254000002,'ÄÝæ«¡¤Â³',63568958253809,0,-1,'',0),(254000003,'AD1270',63568967453981,0,-1,'',0),(254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',63568967471606,0,-1,'',0),(254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',63568967534106,0,-1,'',0),(254000006,'¼§Âê¡¤ÂõÒ®',63568967538215,0,-1,'',0);
/*!40000 ALTER TABLE `t_wanmota` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_warning`
--

DROP TABLE IF EXISTS `t_warning`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_warning` (
  `Id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `usedmoney` int(11) NOT NULL DEFAULT '0' COMMENT 'Ïû·ÑµÄÔª±¦',
  `goodsmoney` int(11) NOT NULL DEFAULT '0' COMMENT 'µÃµ½ÎïÆ·µÄÔª±¦¼ÛÖµ',
  `warningtime` datetime NOT NULL COMMENT '¸æ¾¯Ê±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Ïû·Ñ±¨¾¯¼ÇÂ¼±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_warning`
--

LOCK TABLES `t_warning` WRITE;
/*!40000 ALTER TABLE `t_warning` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_warning` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_wings`
--

DROP TABLE IF EXISTS `t_wings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_wings` (
  `Id` int(11) NOT NULL AUTO_INCREMENT COMMENT '×ÔÔö³¤ID',
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `rname` char(32) DEFAULT NULL,
  `occupation` tinyint(4) NOT NULL COMMENT 'Ö°Òµ',
  `wingid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '×øÆïµÄID',
  `forgeLevel` int(11) NOT NULL COMMENT 'Ç¿»¯´ÎÊý',
  `addtime` datetime NOT NULL COMMENT '¿ªÊ¼ÆôÓÃµÄÊ±¼ä',
  `isdel` tinyint(4) unsigned NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÊÇÒÑ¾­É¾³ý',
  `failednum` int(11) NOT NULL DEFAULT '0' COMMENT '±¾´Î½ø½×³É¹¦Ç°ÒÑ¾­Ê§°ÜµÄ´ÎÊý',
  `equiped` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñÊ¹ÓÃ',
  `starexp` int(11) NOT NULL DEFAULT '0' COMMENT 'ÉýÐÇµÄ¾­ÑéÖµ',
  `zhulingnum` int(11) NOT NULL DEFAULT '0' COMMENT '³á°ò×¢Áé´ÎÊý',
  `zhuhunnum` int(11) NOT NULL DEFAULT '0' COMMENT '³á°ò×¢»ê´ÎÊý',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COMMENT='MU³á°ò±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_wings`
--

LOCK TABLES `t_wings` WRITE;
/*!40000 ALTER TABLE `t_wings` DISABLE KEYS */;
INSERT INTO `t_wings` VALUES (1,254000000,'Âí¿­Î÷¡¤Ê©ÄÍµÂ',1,1,0,'2015-06-03 18:27:11',0,0,0,0,0,0),(2,254000001,'Ê©Æ¤Ëþ¡¤ÑÅ¸÷²¼',1,1,5,'2015-06-03 18:36:06',0,0,1,0,0,0),(3,254000002,'ÄÝæ«¡¤Â³',2,1,0,'2015-06-03 19:57:33',0,0,0,0,0,0),(4,254000003,'AD1270',2,1,0,'2015-06-03 22:30:54',0,0,0,0,0,0),(5,254000004,'Ë¹¿¨À¼¡¤ÃÀÀû',1,1,0,'2015-06-03 22:31:11',0,0,0,0,0,0),(6,254000005,'Ë¹ÌØÀ­¡¤ÂåÅå',0,1,0,'2015-06-03 22:32:14',0,0,0,0,0,0),(7,254000006,'¼§Âê¡¤ÂõÒ®',2,1,0,'2015-06-03 22:32:18',0,0,0,0,0,0);
/*!40000 ALTER TABLE `t_wings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_yabiao`
--

DROP TABLE IF EXISTS `t_yabiao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_yabiao` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `yabiaoid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÑºïÚID',
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00' COMMENT '¿ªÊ¼½ÓïÚµÄÊ±¼ä',
  `state` int(11) NOT NULL DEFAULT '0' COMMENT 'ÑºïÚ×´Ì¬(0:Õý³£, 1:Ê§°Ü)',
  `lineid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÓïÚÊ±µÄÏßÂ·ID',
  `toubao` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñ×öÁËÍ¶±£, 0: Ã»×ö 1:×öÁË',
  `yabiaodayid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÑºïÚµÄÈÕID',
  `yabiaonum` int(11) NOT NULL DEFAULT '0' COMMENT 'Ã¿ÈÕÑºïÚµÄ´ÎÊý',
  `takegoods` int(11) NOT NULL DEFAULT '0' COMMENT 'ÊÇ·ñ½Óµ½ÁË»õÎï',
  UNIQUE KEY `rid` (`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÑºïÚ±í';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_yabiao`
--

LOCK TABLES `t_yabiao` WRITE;
/*!40000 ALTER TABLE `t_yabiao` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_yabiao` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_yangguangbkdailydata`
--

DROP TABLE IF EXISTS `t_yangguangbkdailydata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_yangguangbkdailydata` (
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `jifen` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '»ý·Ö',
  `dayid` int(11) unsigned NOT NULL DEFAULT '0' COMMENT 'ÈÕÆÚID',
  `awardhistory` int(11) unsigned NOT NULL DEFAULT '0' COMMENT '½±ÀøÁìÈ¡ÀúÊ·',
  UNIQUE KEY `rid_unique` (`rid`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='Ñî¹«±¦¿âÃ¿ÈÕÊý¾Ý';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_yangguangbkdailydata`
--

LOCK TABLES `t_yangguangbkdailydata` WRITE;
/*!40000 ALTER TABLE `t_yangguangbkdailydata` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_yangguangbkdailydata` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_yinliangbuy`
--

DROP TABLE IF EXISTS `t_yinliangbuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_yinliangbuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·Ñ',
  `leftyinliang` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàÒøÁ½',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÒøÁ½¹ºÂò¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_yinliangbuy`
--

LOCK TABLES `t_yinliangbuy` WRITE;
/*!40000 ALTER TABLE `t_yinliangbuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_yinliangbuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_yinpiaobuy`
--

DROP TABLE IF EXISTS `t_yinpiaobuy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_yinpiaobuy` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `goodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ID',
  `goodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'ÎïÆ·ÊýÁ¿',
  `totalprice` int(11) NOT NULL DEFAULT '0' COMMENT '×Ü»¨·ÑÒøÆ±¸öÊý',
  `leftyinpiao` int(11) NOT NULL DEFAULT '0' COMMENT 'Ê£ÓàÒøÆ±',
  `buytime` datetime NOT NULL COMMENT '¹ºÂòÊ±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÒøÆ±¹ºÂò¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_yinpiaobuy`
--

LOCK TABLES `t_yinpiaobuy` WRITE;
/*!40000 ALTER TABLE `t_yinpiaobuy` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_yinpiaobuy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_yueduchoujianghist`
--

DROP TABLE IF EXISTS `t_yueduchoujianghist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_yueduchoujianghist` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0',
  `rname` char(32) DEFAULT NULL,
  `zoneid` int(11) NOT NULL DEFAULT '0',
  `gaingoodsid` int(11) NOT NULL DEFAULT '0',
  `gaingoodsnum` int(11) NOT NULL DEFAULT '0',
  `gaingold` int(11) NOT NULL DEFAULT '0',
  `gainyinliang` int(11) NOT NULL DEFAULT '0',
  `gainexp` int(11) NOT NULL DEFAULT '0',
  `operationtime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='�¶ȳ齱��ʷ��¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_yueduchoujianghist`
--

LOCK TABLES `t_yueduchoujianghist` WRITE;
/*!40000 ALTER TABLE `t_yueduchoujianghist` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_yueduchoujianghist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_zajindanhist`
--

DROP TABLE IF EXISTS `t_zajindanhist`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_zajindanhist` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ID',
  `rname` char(32) DEFAULT NULL,
  `zoneid` int(11) NOT NULL DEFAULT '0' COMMENT '½ÇÉ«ÇøºÅ',
  `timesselected` int(11) NOT NULL DEFAULT '0' COMMENT '´ÎÊýÑ¡Ôñ',
  `usedyuanbao` int(11) NOT NULL DEFAULT '0' COMMENT 'ÏûºÄÔª±¦',
  `usedjindan` int(11) NOT NULL DEFAULT '0' COMMENT 'ÏûºÄ½ðµ°¸öÊý',
  `gaingoodsid` int(11) NOT NULL DEFAULT '0' COMMENT 'µÃµ½ÎïÆ·id',
  `gaingoodsnum` int(11) NOT NULL DEFAULT '0' COMMENT 'µÃµ½ÎïÆ·ÊýÁ¿[Ò»¸ö]',
  `gaingold` int(11) NOT NULL DEFAULT '0' COMMENT 'µÃµ½½ð±Ò',
  `gainyinliang` int(11) NOT NULL DEFAULT '0' COMMENT 'µÃµ½ÒøÁ½',
  `gainexp` int(11) NOT NULL DEFAULT '0' COMMENT 'µÃµ½¾­Ñé',
  `strprop` char(255) NOT NULL,
  `operationtime` datetime NOT NULL COMMENT 'ÔÒ½ðµ°²Ù×÷Ê±¼ä',
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COMMENT='ÔÒ½ðµ°ÀúÊ·¼ÇÂ¼';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_zajindanhist`
--

LOCK TABLES `t_zajindanhist` WRITE;
/*!40000 ALTER TABLE `t_zajindanhist` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_zajindanhist` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `t_zhanmengshijian`
--

DROP TABLE IF EXISTS `t_zhanmengshijian`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_zhanmengshijian` (
  `pkId` int(11) NOT NULL AUTO_INCREMENT COMMENT 'Ö÷¼ü',
  `bhId` int(11) NOT NULL COMMENT '»á°ïID',
  `shijianType` int(11) NOT NULL COMMENT 'ÊÂ¼þÀàÐÍ',
  `roleName` char(32) DEFAULT NULL,
  `createTime` datetime NOT NULL COMMENT 'ÊÂ¼þ·¢ÉúÊ±¼ä',
  `subValue1` int(11) NOT NULL COMMENT 'Ô¤ÁôÖµ',
  `subValue2` int(11) NOT NULL COMMENT 'Ô¤ÁôÖµ',
  `subValue3` int(11) NOT NULL COMMENT 'Ô¤ÁôÖµ',
  PRIMARY KEY (`pkId`),
  KEY `idx_t_zhanmengshijian_bhId` (`bhId`),
  KEY `idx_t_zhanmengshijian_createTime` (`createTime`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `t_zhanmengshijian`
--

LOCK TABLES `t_zhanmengshijian` WRITE;
/*!40000 ALTER TABLE `t_zhanmengshijian` DISABLE KEYS */;
/*!40000 ALTER TABLE `t_zhanmengshijian` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `test`
--

DROP TABLE IF EXISTS `test`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `test` (
  `Id` int(6) unsigned NOT NULL AUTO_INCREMENT,
  `name` char(30) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `test`
--

LOCK TABLES `test` WRITE;
/*!40000 ALTER TABLE `test` DISABLE KEYS */;
/*!40000 ALTER TABLE `test` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-06-04  2:23:03
