/*
Navicat MySQL Data Transfer

Source Server         : 192.168.1.253
Source Server Version : 50540
Source Host           : localhost:3306
Source Database       : dynasty

Target Server Type    : MYSQL
Target Server Version : 50540
File Encoding         : 65001

Date: 2016-06-26 17:42:22
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for account
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account` (
  `accId` bigint(16) NOT NULL,
  `accName` varchar(20) NOT NULL,
  `createDttm` timestamp NULL DEFAULT NULL,
  `loginIp` varchar(20) DEFAULT NULL,
  `lastLoginDttm` timestamp NULL DEFAULT NULL,
  `status` int(2) DEFAULT '0',
  `statusDec` varchar(60) DEFAULT NULL,
  `partnerId` varchar(16) DEFAULT NULL,
  `allUserId` varchar(200) DEFAULT '',
  `accDesc` varchar(40) DEFAULT '',
  `envelopDttm` timestamp NULL DEFAULT NULL COMMENT '????',
  `qqVipFlag` int(4) DEFAULT '0' COMMENT 'QQVip(0-????;1-vip??;2-????)',
  `qqVipLv` int(4) DEFAULT '0' COMMENT 'vip????',
  `blueVipFlag` int(4) DEFAULT '0' COMMENT '??Vip??(0-????;1-vip??;2-????)',
  `blueVipLv` int(4) DEFAULT '0' COMMENT '??????',
  `yellowVipFlag` int(4) DEFAULT '0' COMMENT '??Vip??(0-????;1-vip??;2-????)',
  `yellowVipLv` int(4) DEFAULT '0' COMMENT '??????',
  `userName` varchar(20) DEFAULT NULL COMMENT '???',
  `inviteAccId` bigint(16) DEFAULT '0' COMMENT '??????',
  PRIMARY KEY (`accId`),
  UNIQUE KEY `accName` (`accName`),
  KEY `index_Account_accId` (`accId`),
  KEY `userName` (`userName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of account
-- ----------------------------

-- ----------------------------
-- Table structure for achieve
-- ----------------------------
DROP TABLE IF EXISTS `achieve`;
CREATE TABLE `achieve` (
  `achieveId` int(16) NOT NULL AUTO_INCREMENT,
  `name` varchar(40) DEFAULT NULL,
  `detail` varchar(500) DEFAULT NULL,
  `type` varchar(40) DEFAULT NULL,
  `childType` varchar(40) DEFAULT NULL,
  `achieveValue` int(16) DEFAULT '0',
  `para1` int(16) DEFAULT '0',
  `para2` int(16) DEFAULT '0',
  `para3` int(16) DEFAULT '0',
  `level` int(8) DEFAULT '1' COMMENT '?????????????',
  PRIMARY KEY (`achieveId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????--???????';

-- ----------------------------
-- Records of achieve
-- ----------------------------

-- ----------------------------
-- Table structure for actionmission
-- ----------------------------
DROP TABLE IF EXISTS `actionmission`;
CREATE TABLE `actionmission` (
  `missionId` int(16) NOT NULL DEFAULT '0' COMMENT 'id(80000-90000??)',
  `name` varchar(20) NOT NULL COMMENT '??',
  `actionTime` varchar(40) NOT NULL COMMENT '????(yyyy-mm-dd hh:mm:ss|yyyy-mm-dd hh:mm:ss)',
  `missionDesc` varchar(500) NOT NULL COMMENT '??',
  `guide` varchar(500) NOT NULL COMMENT '??',
  `missionDex` varchar(500) NOT NULL COMMENT '????(??1|??2|??3)',
  `para1` varchar(500) NOT NULL COMMENT '??1',
  `para2` varchar(500) NOT NULL COMMENT '??2',
  `para3` varchar(500) NOT NULL COMMENT '??3',
  `scriptName` varchar(500) NOT NULL COMMENT '???',
  `item` varchar(200) NOT NULL COMMENT '????(??id,??;??id,??;)',
  `showFlag` int(2) DEFAULT '0' COMMENT '????(0-??? 1-??)',
  `name2` varchar(100) DEFAULT NULL COMMENT '???????????',
  PRIMARY KEY (`missionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of actionmission
-- ----------------------------

-- ----------------------------
-- Table structure for actiontip
-- ----------------------------
DROP TABLE IF EXISTS `actiontip`;
CREATE TABLE `actiontip` (
  `name` varchar(20) NOT NULL,
  `showName` varchar(100) DEFAULT NULL,
  `actionDesc` varchar(200) DEFAULT NULL,
  `awardDesc` varchar(200) DEFAULT NULL,
  `type` int(2) DEFAULT '0',
  `fightType` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of actiontip
-- ----------------------------

-- ----------------------------
-- Table structure for activity
-- ----------------------------
DROP TABLE IF EXISTS `activity`;
CREATE TABLE `activity` (
  `actId` int(16) NOT NULL AUTO_INCREMENT COMMENT '???????',
  `actDesc` varchar(200) DEFAULT NULL COMMENT '???????',
  `needNum` int(8) DEFAULT '0' COMMENT '?????',
  `unit` varchar(20) DEFAULT NULL COMMENT '????(? ? ?)',
  `showGiftTip` varchar(500) DEFAULT NULL COMMENT '????',
  PRIMARY KEY (`actId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of activity
-- ----------------------------

-- ----------------------------
-- Table structure for activityaddpoint
-- ----------------------------
DROP TABLE IF EXISTS `activityaddpoint`;
CREATE TABLE `activityaddpoint` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `actId` int(16) DEFAULT '0' COMMENT '???????',
  `addNumDesc` varchar(200) DEFAULT NULL COMMENT '?????? 5+10+15',
  `completeNum` int(4) DEFAULT '0' COMMENT '????',
  `addNum` int(4) DEFAULT '0' COMMENT '????',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????????';

-- ----------------------------
-- Records of activityaddpoint
-- ----------------------------

-- ----------------------------
-- Table structure for activitybox
-- ----------------------------
DROP TABLE IF EXISTS `activitybox`;
CREATE TABLE `activitybox` (
  `boxId` int(16) NOT NULL AUTO_INCREMENT COMMENT '???? 1001 - 1004',
  `entId` int(16) DEFAULT '0' COMMENT '??id',
  `num` int(4) DEFAULT '0' COMMENT '????',
  `needActitityPoint` int(4) DEFAULT '0' COMMENT '??????',
  PRIMARY KEY (`boxId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????';

-- ----------------------------
-- Records of activitybox
-- ----------------------------

-- ----------------------------
-- Table structure for activiylock
-- ----------------------------
DROP TABLE IF EXISTS `activiylock`;
CREATE TABLE `activiylock` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `level` int(2) DEFAULT '0',
  `type` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_level` (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???';

-- ----------------------------
-- Records of activiylock
-- ----------------------------

-- ----------------------------
-- Table structure for activiymessage
-- ----------------------------
DROP TABLE IF EXISTS `activiymessage`;
CREATE TABLE `activiymessage` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `type` int(2) DEFAULT '0',
  `winNum` int(4) DEFAULT '0',
  `mes` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of activiymessage
-- ----------------------------

-- ----------------------------
-- Table structure for answerresult
-- ----------------------------
DROP TABLE IF EXISTS `answerresult`;
CREATE TABLE `answerresult` (
  `code` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) NOT NULL,
  `examCode` int(16) NOT NULL,
  `resultFlag` char(1) NOT NULL,
  `createTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `type` int(1) NOT NULL,
  `ringFlag` bigint(20) NOT NULL,
  `itemId` int(16) NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of answerresult
-- ----------------------------

-- ----------------------------
-- Table structure for arena
-- ----------------------------
DROP TABLE IF EXISTS `arena`;
CREATE TABLE `arena` (
  `arenaId` int(16) NOT NULL,
  `arenaLv` int(2) DEFAULT '0',
  `heroId` int(16) DEFAULT '0',
  `userId` int(16) DEFAULT '0',
  `joinDttm` timestamp NULL DEFAULT NULL,
  `poolExp` int(8) DEFAULT '0',
  `winNum` int(4) DEFAULT '0',
  `bakHeroId` varchar(20) DEFAULT NULL,
  `merit` int(16) DEFAULT '0',
  PRIMARY KEY (`arenaId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of arena
-- ----------------------------

-- ----------------------------
-- Table structure for arenarecord
-- ----------------------------
DROP TABLE IF EXISTS `arenarecord`;
CREATE TABLE `arenarecord` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) NOT NULL,
  `heroId` int(16) NOT NULL,
  `operType` varchar(40) DEFAULT NULL,
  `operDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of arenarecord
-- ----------------------------

-- ----------------------------
-- Table structure for army
-- ----------------------------
DROP TABLE IF EXISTS `army`;
CREATE TABLE `army` (
  `entId` int(16) NOT NULL,
  `armyName` varchar(20) NOT NULL DEFAULT '0',
  `armyDesc` varchar(100) NOT NULL,
  `armyType` varchar(40) NOT NULL,
  `iconPath` varchar(100) NOT NULL,
  `attack` int(4) NOT NULL DEFAULT '0',
  `defence` int(4) NOT NULL DEFAULT '0',
  `agile` int(4) NOT NULL DEFAULT '0',
  `hp` int(4) NOT NULL DEFAULT '0',
  `level` int(2) NOT NULL DEFAULT '0',
  `attackType` varchar(20) NOT NULL COMMENT '????',
  `defenceType` varchar(20) NOT NULL COMMENT '????',
  `moveRange` int(4) NOT NULL DEFAULT '0' COMMENT '???,?????????????',
  `mobility` int(4) NOT NULL DEFAULT '0' COMMENT '???,????????????',
  `convey` int(8) NOT NULL DEFAULT '0' COMMENT '???,??????????',
  `hitRate` int(4) NOT NULL DEFAULT '0' COMMENT '???',
  `dodgeRate` int(4) NOT NULL DEFAULT '0' COMMENT '???',
  `criticalAtkRate` int(4) NOT NULL DEFAULT '0' COMMENT '???',
  `caromAtkRate` int(4) NOT NULL DEFAULT '0' COMMENT '???',
  `counterAtkRate` int(4) NOT NULL DEFAULT '0' COMMENT '???',
  `attackRange` int(4) NOT NULL DEFAULT '0' COMMENT '??????,???1',
  `consumeFood` int(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of army
-- ----------------------------

-- ----------------------------
-- Table structure for armyattackorder
-- ----------------------------
DROP TABLE IF EXISTS `armyattackorder`;
CREATE TABLE `armyattackorder` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `atkArmyType` varchar(20) NOT NULL,
  `dfcArmyType` varchar(20) NOT NULL,
  `orderId` int(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of armyattackorder
-- ----------------------------

-- ----------------------------
-- Table structure for armyovercome
-- ----------------------------
DROP TABLE IF EXISTS `armyovercome`;
CREATE TABLE `armyovercome` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `armyAttackType` varchar(20) NOT NULL,
  `armyDefenceType` varchar(20) NOT NULL,
  `factor` double(8,2) NOT NULL DEFAULT '0.00',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of armyovercome
-- ----------------------------

-- ----------------------------
-- Table structure for attackout
-- ----------------------------
DROP TABLE IF EXISTS `attackout`;
CREATE TABLE `attackout` (
  `atkOutId` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) NOT NULL,
  `targetCasId` int(16) DEFAULT '0',
  `outArriveDttm` timestamp NULL DEFAULT NULL,
  `outBackDttm` timestamp NULL DEFAULT NULL,
  `type` varchar(40) DEFAULT NULL,
  `combatId` varchar(100) DEFAULT NULL,
  `heroIds` varchar(100) DEFAULT NULL,
  `baseTime` int(16) DEFAULT '0',
  `armyOutDttm` timestamp NULL DEFAULT NULL,
  `status` int(2) DEFAULT '0',
  `troopId` int(16) DEFAULT '0',
  `outPower` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`atkOutId`),
  KEY `index_AttackOut_casId` (`casId`),
  KEY `index_AttackOut_targetCasId` (`targetCasId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of attackout
-- ----------------------------

-- ----------------------------
-- Table structure for authority
-- ----------------------------
DROP TABLE IF EXISTS `authority`;
CREATE TABLE `authority` (
  `code` int(4) NOT NULL AUTO_INCREMENT,
  `path` varchar(60) NOT NULL,
  `name` varchar(60) NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of authority
-- ----------------------------

-- ----------------------------
-- Table structure for awardcasfactor
-- ----------------------------
DROP TABLE IF EXISTS `awardcasfactor`;
CREATE TABLE `awardcasfactor` (
  `id` int(16) NOT NULL DEFAULT '0',
  `factor` double(8,2) DEFAULT '0.00',
  `minCasRange` int(16) DEFAULT '0',
  `maxCasRange` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????????????';

-- ----------------------------
-- Records of awardcasfactor
-- ----------------------------

-- ----------------------------
-- Table structure for awardluckfactor
-- ----------------------------
DROP TABLE IF EXISTS `awardluckfactor`;
CREATE TABLE `awardluckfactor` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `luckName` varchar(40) DEFAULT NULL,
  `luckDesc` varchar(200) DEFAULT NULL,
  `factor` double(8,2) DEFAULT '0.00',
  `resWord` varchar(200) DEFAULT NULL,
  `icon` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????';

-- ----------------------------
-- Records of awardluckfactor
-- ----------------------------

-- ----------------------------
-- Table structure for babelawardrecord
-- ----------------------------
DROP TABLE IF EXISTS `babelawardrecord`;
CREATE TABLE `babelawardrecord` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `stageId` int(8) DEFAULT '0',
  `itemId` int(16) DEFAULT '0',
  `itemNum` int(8) DEFAULT '1',
  `itemName` varchar(100) DEFAULT NULL,
  `recordDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of babelawardrecord
-- ----------------------------

-- ----------------------------
-- Table structure for babelround
-- ----------------------------
DROP TABLE IF EXISTS `babelround`;
CREATE TABLE `babelround` (
  `roundId` int(16) NOT NULL AUTO_INCREMENT,
  `roundName` varchar(20) DEFAULT NULL,
  `roundDesc` varchar(800) DEFAULT NULL,
  `iconPath` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`roundId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???(??)';

-- ----------------------------
-- Records of babelround
-- ----------------------------

-- ----------------------------
-- Table structure for babelscorefactor
-- ----------------------------
DROP TABLE IF EXISTS `babelscorefactor`;
CREATE TABLE `babelscorefactor` (
  `resultScore` int(4) NOT NULL DEFAULT '0',
  `chsName` varchar(20) DEFAULT NULL,
  `scoreFactor` double(8,2) DEFAULT '0.00',
  PRIMARY KEY (`resultScore`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???????';

-- ----------------------------
-- Records of babelscorefactor
-- ----------------------------

-- ----------------------------
-- Table structure for babelstage
-- ----------------------------
DROP TABLE IF EXISTS `babelstage`;
CREATE TABLE `babelstage` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `stageId` int(16) DEFAULT '0',
  `stageDesc` varchar(800) DEFAULT NULL,
  `monsterId` int(16) DEFAULT '0',
  `heroExp` int(16) DEFAULT '0',
  `babelScore` int(16) DEFAULT '0',
  `reliveRate` int(4) DEFAULT '0',
  `awardStr` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???(??) ';

-- ----------------------------
-- Records of babelstage
-- ----------------------------

-- ----------------------------
-- Table structure for battledex
-- ----------------------------
DROP TABLE IF EXISTS `battledex`;
CREATE TABLE `battledex` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `posX` int(4) DEFAULT '0',
  `posY` int(4) DEFAULT '0',
  `casId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of battledex
-- ----------------------------

-- ----------------------------
-- Table structure for battleplan
-- ----------------------------
DROP TABLE IF EXISTS `battleplan`;
CREATE TABLE `battleplan` (
  `casId` int(16) NOT NULL DEFAULT '0',
  `hero11` int(16) DEFAULT '0',
  `hero12` int(16) DEFAULT '0',
  `hero13` int(16) DEFAULT '0',
  `hero14` int(16) DEFAULT '0',
  `hero15` int(16) DEFAULT '0',
  `hero21` int(16) DEFAULT '0',
  `hero22` int(16) DEFAULT '0',
  `hero23` int(16) DEFAULT '0',
  `hero24` int(16) DEFAULT '0',
  `hero25` int(16) DEFAULT '0',
  `hero31` int(16) DEFAULT '0',
  `hero32` int(16) DEFAULT '0',
  `hero33` int(16) DEFAULT '0',
  `hero34` int(16) DEFAULT '0',
  `hero35` int(16) DEFAULT '0',
  PRIMARY KEY (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of battleplan
-- ----------------------------

-- ----------------------------
-- Table structure for boxaward
-- ----------------------------
DROP TABLE IF EXISTS `boxaward`;
CREATE TABLE `boxaward` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `boxId` int(16) DEFAULT '0',
  `boxName` varchar(100) DEFAULT NULL,
  `awardName` varchar(100) DEFAULT NULL,
  `type` int(2) DEFAULT '0',
  `objId` int(16) DEFAULT '0',
  `objNum` int(16) DEFAULT '0',
  `weight` int(16) DEFAULT '0',
  `picPath` varchar(100) DEFAULT NULL,
  `needSendMsg` int(2) DEFAULT '0' COMMENT '??????????????(1???,0???)',
  `wordMsg` varchar(200) DEFAULT NULL COMMENT '??????????',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of boxaward
-- ----------------------------

-- ----------------------------
-- Table structure for bufficon
-- ----------------------------
DROP TABLE IF EXISTS `bufficon`;
CREATE TABLE `bufficon` (
  `buffName` varchar(40) NOT NULL,
  `iconPath` varchar(40) DEFAULT NULL,
  `buffDesc` varchar(100) DEFAULT NULL,
  `type` int(2) DEFAULT '0',
  `remainSecond` int(8) DEFAULT '0',
  PRIMARY KEY (`buffName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of bufficon
-- ----------------------------

-- ----------------------------
-- Table structure for bufftip
-- ----------------------------
DROP TABLE IF EXISTS `bufftip`;
CREATE TABLE `bufftip` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `casId` int(16) DEFAULT '0',
  `endTime` timestamp NULL DEFAULT NULL,
  `buffName` varchar(40) DEFAULT NULL,
  `type` int(2) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `index_BuffTip_userId` (`userId`),
  KEY `index_BuffTip_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of bufftip
-- ----------------------------

-- ----------------------------
-- Table structure for buildcastleconsume
-- ----------------------------
DROP TABLE IF EXISTS `buildcastleconsume`;
CREATE TABLE `buildcastleconsume` (
  `no` int(16) NOT NULL AUTO_INCREMENT,
  `userTitle` int(4) DEFAULT '0',
  `wood` int(8) DEFAULT '0',
  `food` int(8) DEFAULT '0',
  `stone` int(8) DEFAULT '0',
  `bronze` int(8) DEFAULT '0',
  `money` int(8) DEFAULT '0',
  `itemNum` int(4) DEFAULT '0',
  PRIMARY KEY (`no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of buildcastleconsume
-- ----------------------------

-- ----------------------------
-- Table structure for building
-- ----------------------------
DROP TABLE IF EXISTS `building`;
CREATE TABLE `building` (
  `entId` int(16) NOT NULL,
  `buiName` varchar(40) DEFAULT NULL,
  `buiDesc` varchar(200) DEFAULT NULL,
  `iconPath` varchar(100) DEFAULT NULL,
  `numLimit` int(2) DEFAULT '0',
  `maxLevel` int(4) DEFAULT '0',
  `property` varchar(40) DEFAULT NULL,
  `castleType` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of building
-- ----------------------------

-- ----------------------------
-- Table structure for buildingqueue
-- ----------------------------
DROP TABLE IF EXISTS `buildingqueue`;
CREATE TABLE `buildingqueue` (
  `queId` int(16) NOT NULL AUTO_INCREMENT,
  `casBuiId` int(16) NOT NULL,
  `time` int(8) NOT NULL DEFAULT '0',
  `isUpgrade` int(2) NOT NULL DEFAULT '0',
  `opDttm` timestamp NULL DEFAULT NULL,
  `queType` varchar(20) NOT NULL,
  `memo` varchar(100) DEFAULT NULL,
  `userId` int(16) DEFAULT '0',
  `casId` int(8) DEFAULT '0',
  `buiEntId` int(16) DEFAULT '0',
  `posNo` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`queId`),
  KEY `index_BuildingQueue_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of buildingqueue
-- ----------------------------

-- ----------------------------
-- Table structure for career
-- ----------------------------
DROP TABLE IF EXISTS `career`;
CREATE TABLE `career` (
  `careerId` int(2) NOT NULL DEFAULT '0',
  `careerName` varchar(20) DEFAULT NULL,
  `careerEntName` varchar(20) DEFAULT NULL,
  `buBing` double(8,2) DEFAULT '0.00',
  `gongBing` double(8,2) DEFAULT NULL,
  `cheBing` double(8,2) DEFAULT NULL,
  `qiBing` double(8,2) DEFAULT NULL,
  `qiXie` double(8,2) DEFAULT NULL,
  PRIMARY KEY (`careerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of career
-- ----------------------------

-- ----------------------------
-- Table structure for cashcredit
-- ----------------------------
DROP TABLE IF EXISTS `cashcredit`;
CREATE TABLE `cashcredit` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `sourceQQUIN` varchar(20) DEFAULT NULL COMMENT 'QQ? UIN (???Q?)',
  `svrId` varchar(10) DEFAULT NULL COMMENT '???',
  `targetQQUIN` varchar(20) DEFAULT NULL COMMENT '??QQ? UIN  (??????Q?)',
  `userId` int(16) NOT NULL COMMENT '??????Q??userId',
  `channel` varchar(10) DEFAULT NULL COMMENT '????',
  `creditNum` int(16) NOT NULL COMMENT '????',
  `orderId` varchar(100) DEFAULT NULL COMMENT '??ID',
  `creditDttm` timestamp NULL DEFAULT NULL COMMENT '????',
  `para` varchar(100) DEFAULT NULL COMMENT '?????',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of cashcredit
-- ----------------------------

-- ----------------------------
-- Table structure for caslvmaxposno
-- ----------------------------
DROP TABLE IF EXISTS `caslvmaxposno`;
CREATE TABLE `caslvmaxposno` (
  `casLv` int(2) NOT NULL DEFAULT '0',
  `maxAPosNo` int(2) NOT NULL DEFAULT '0',
  `maxBPosNo` int(2) NOT NULL DEFAULT '0',
  `maxCPosNo` int(2) NOT NULL DEFAULT '0',
  `castleType` varchar(20) NOT NULL,
  PRIMARY KEY (`casLv`,`castleType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of caslvmaxposno
-- ----------------------------

-- ----------------------------
-- Table structure for castle
-- ----------------------------
DROP TABLE IF EXISTS `castle`;
CREATE TABLE `castle` (
  `casId` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) NOT NULL,
  `casName` varchar(40) DEFAULT NULL,
  `casLv` int(4) DEFAULT '0',
  `rangeValue` int(8) DEFAULT '0',
  `rangeCeil` int(8) DEFAULT '0',
  `rangeLv` int(4) DEFAULT '0',
  `calcuDttm` timestamp NULL DEFAULT NULL,
  `quarCalcuDttm` timestamp NULL DEFAULT NULL,
  `stateId` int(16) DEFAULT '0',
  `operPeopleDttm` timestamp NULL DEFAULT NULL,
  `preArmyNum` int(8) DEFAULT '0',
  `recruitDttm` timestamp NULL DEFAULT NULL,
  `posX` int(4) DEFAULT '0',
  `posY` int(4) DEFAULT '0',
  `status` int(2) DEFAULT '0',
  `robbedTimes` int(2) DEFAULT '0',
  `todayLost` int(16) DEFAULT '0',
  `castleType` varchar(20) DEFAULT NULL,
  `parentCasId` int(8) DEFAULT '0',
  `branchCasTypeList` varchar(100) DEFAULT NULL,
  `occuFlagTime` timestamp NULL DEFAULT NULL,
  `addHonorMissionDttm` timestamp NULL DEFAULT NULL,
  `addCurrLv` varchar(20) DEFAULT NULL,
  `changeCountryDttm` timestamp NULL DEFAULT NULL COMMENT '???????',
  `qqType` varchar(20) DEFAULT NULL COMMENT '????',
  `qqEndTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  PRIMARY KEY (`casId`),
  KEY `index_Castle_userId` (`userId`),
  KEY `index_Castle_rangeValue` (`rangeValue`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castle
-- ----------------------------

-- ----------------------------
-- Table structure for castlearmy
-- ----------------------------
DROP TABLE IF EXISTS `castlearmy`;
CREATE TABLE `castlearmy` (
  `casArmyId` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) NOT NULL DEFAULT '0',
  `armyEntId` int(16) NOT NULL,
  `num` int(8) NOT NULL,
  PRIMARY KEY (`casArmyId`),
  KEY `index_CastleArmy_casId` (`casId`),
  KEY `index_CastleArmy_armyEntId` (`armyEntId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castlearmy
-- ----------------------------

-- ----------------------------
-- Table structure for castlebattlelevel
-- ----------------------------
DROP TABLE IF EXISTS `castlebattlelevel`;
CREATE TABLE `castlebattlelevel` (
  `level` int(4) NOT NULL DEFAULT '0',
  `atk` int(8) DEFAULT '0',
  `def` int(8) DEFAULT '0',
  `hp` int(8) DEFAULT '0',
  `agl` int(8) DEFAULT '0',
  `lead` int(8) DEFAULT '0',
  `wallHp` int(8) DEFAULT '0',
  `wallDef` int(8) DEFAULT '0',
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castlebattlelevel
-- ----------------------------

-- ----------------------------
-- Table structure for castlebattleunit
-- ----------------------------
DROP TABLE IF EXISTS `castlebattleunit`;
CREATE TABLE `castlebattleunit` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) DEFAULT '0',
  `unitIndex` int(2) DEFAULT '0',
  `armyEntId` int(16) DEFAULT '0',
  `armyNum` int(8) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `index_CastleBattleUnit_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castlebattleunit
-- ----------------------------

-- ----------------------------
-- Table structure for castlebuilding
-- ----------------------------
DROP TABLE IF EXISTS `castlebuilding`;
CREATE TABLE `castlebuilding` (
  `casBuiId` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) NOT NULL,
  `buiEntId` int(16) NOT NULL,
  `level` int(4) DEFAULT '0',
  `posNo` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`casBuiId`),
  KEY `index_CastleBuilding_casId` (`casId`),
  KEY `index_CastleBuilding_buiEntId` (`buiEntId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castlebuilding
-- ----------------------------

-- ----------------------------
-- Table structure for castleeffect
-- ----------------------------
DROP TABLE IF EXISTS `castleeffect`;
CREATE TABLE `castleeffect` (
  `playerEffId` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) DEFAULT '0' COMMENT '??id',
  `effectId` varchar(40) DEFAULT NULL COMMENT '??id',
  `type` varchar(40) DEFAULT NULL COMMENT '????(?????...)',
  `itemEffectId` int(8) DEFAULT '0' COMMENT '????id',
  `absValue` int(8) DEFAULT '0' COMMENT '?????',
  `perValue` int(8) DEFAULT '0' COMMENT '?????',
  `showFlag` int(2) DEFAULT '0' COMMENT '(?????)',
  `expireDttm` timestamp NULL DEFAULT NULL COMMENT '??????',
  PRIMARY KEY (`playerEffId`),
  KEY `index_CaslteEffect_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castleeffect
-- ----------------------------

-- ----------------------------
-- Table structure for castlepop
-- ----------------------------
DROP TABLE IF EXISTS `castlepop`;
CREATE TABLE `castlepop` (
  `casId` int(16) NOT NULL,
  `idlePop` int(16) DEFAULT '0',
  `foodPop` int(16) DEFAULT '0',
  `woodPop` int(16) DEFAULT '0',
  `stonePop` int(16) DEFAULT '0',
  `bronzePop` int(16) DEFAULT '0',
  PRIMARY KEY (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castlepop
-- ----------------------------

-- ----------------------------
-- Table structure for castleres
-- ----------------------------
DROP TABLE IF EXISTS `castleres`;
CREATE TABLE `castleres` (
  `casId` int(16) NOT NULL,
  `foodNum` int(16) DEFAULT '0',
  `woodNum` int(16) DEFAULT '0',
  `stoneNum` int(16) DEFAULT '0',
  `bronzeNum` int(16) DEFAULT '0',
  `ironNum` int(16) DEFAULT '0',
  `moneyNum` int(16) DEFAULT '0',
  `cashNum` int(4) DEFAULT '0',
  `lastBuyDttm` timestamp NULL DEFAULT NULL,
  `changeExpNum` int(4) DEFAULT '0',
  `lastChangeDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castleres
-- ----------------------------

-- ----------------------------
-- Table structure for castletech
-- ----------------------------
DROP TABLE IF EXISTS `castletech`;
CREATE TABLE `castletech` (
  `casTechId` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) NOT NULL,
  `techEntId` int(16) NOT NULL,
  `level` int(4) DEFAULT '0',
  `opDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`casTechId`),
  KEY `index_CastleTech_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castletech
-- ----------------------------

-- ----------------------------
-- Table structure for castletowerarmy
-- ----------------------------
DROP TABLE IF EXISTS `castletowerarmy`;
CREATE TABLE `castletowerarmy` (
  `casTowerArmyId` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) DEFAULT '0',
  `towerArmyEntId` int(16) DEFAULT '0',
  `num` int(8) DEFAULT '0',
  PRIMARY KEY (`casTowerArmyId`),
  KEY `index_CastleTowerArmy_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castletowerarmy
-- ----------------------------

-- ----------------------------
-- Table structure for castletowerarmyqueue
-- ----------------------------
DROP TABLE IF EXISTS `castletowerarmyqueue`;
CREATE TABLE `castletowerarmyqueue` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) DEFAULT '0',
  `entId` int(16) DEFAULT '0',
  `buiNum` int(8) DEFAULT '0',
  `time` int(8) DEFAULT '0',
  `startTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of castletowerarmyqueue
-- ----------------------------

-- ----------------------------
-- Table structure for charaspel
-- ----------------------------
DROP TABLE IF EXISTS `charaspel`;
CREATE TABLE `charaspel` (
  `chara` int(16) NOT NULL DEFAULT '0',
  `feng` varchar(20) DEFAULT NULL,
  `lin` varchar(20) DEFAULT NULL,
  `huo` varchar(20) DEFAULT NULL,
  `shan` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`chara`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of charaspel
-- ----------------------------

-- ----------------------------
-- Table structure for country
-- ----------------------------
DROP TABLE IF EXISTS `country`;
CREATE TABLE `country` (
  `countryId` int(16) NOT NULL AUTO_INCREMENT,
  `countryName` varchar(100) NOT NULL,
  PRIMARY KEY (`countryId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of country
-- ----------------------------

-- ----------------------------
-- Table structure for dailygift
-- ----------------------------
DROP TABLE IF EXISTS `dailygift`;
CREATE TABLE `dailygift` (
  `id` int(2) NOT NULL DEFAULT '0',
  `rankId` int(2) DEFAULT '0',
  `userExp` int(8) DEFAULT '0',
  `userPoint` int(4) DEFAULT '0',
  `payPoint` int(4) DEFAULT '0',
  `resouce` int(8) DEFAULT '0',
  `money` int(8) DEFAULT '0',
  `itemId` int(16) DEFAULT '0',
  `itemNum` int(4) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dailygift
-- ----------------------------

-- ----------------------------
-- Table structure for dailymission
-- ----------------------------
DROP TABLE IF EXISTS `dailymission`;
CREATE TABLE `dailymission` (
  `dmId` int(16) NOT NULL DEFAULT '0',
  `iconPath` varchar(20) DEFAULT NULL,
  `name` varchar(20) DEFAULT NULL,
  `missionDesc` varchar(200) DEFAULT NULL,
  `detail` varchar(200) DEFAULT NULL,
  `reUserProperty` varchar(20) DEFAULT NULL,
  `reUserValue` int(4) DEFAULT '0',
  `requireActPoint` int(4) DEFAULT '0',
  `missionActTime` int(16) DEFAULT '0',
  `addUserHonor` int(4) DEFAULT '0',
  `addUserProPointMin` int(2) DEFAULT '0',
  `addUserProPointMiddle` int(2) DEFAULT '0',
  `addUserProPointMax` int(2) DEFAULT '0',
  `food` int(8) DEFAULT '0',
  `money` int(8) DEFAULT '0',
  `idlePop` int(4) DEFAULT '0',
  `newArmy` int(4) DEFAULT '0',
  `towerArmyNum` int(4) DEFAULT '0',
  `dropPackId` int(16) DEFAULT '0',
  `resultMin` varchar(200) DEFAULT NULL,
  `resultMid` varchar(200) DEFAULT NULL,
  `resultMax` varchar(200) DEFAULT NULL,
  `coldDownTime` int(8) DEFAULT '0',
  `rankId` int(16) DEFAULT '0',
  PRIMARY KEY (`dmId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dailymission
-- ----------------------------

-- ----------------------------
-- Table structure for dailymovement
-- ----------------------------
DROP TABLE IF EXISTS `dailymovement`;
CREATE TABLE `dailymovement` (
  `userId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `guildTradeCnt` int(8) DEFAULT '0' COMMENT '??????',
  `userAnswerNum` int(8) DEFAULT '0' COMMENT '??????',
  `usedActivityPoint` int(8) DEFAULT '0' COMMENT '??????',
  `login` int(8) DEFAULT '0' COMMENT '??????',
  `useChangeLuck` int(8) DEFAULT '0' COMMENT '???????',
  `randomEvent` int(8) DEFAULT '0' COMMENT '??????',
  `farmHarvestTree` int(8) DEFAULT '0' COMMENT '???????',
  `farmHarvestAll` int(8) DEFAULT '0' COMMENT '????????',
  `farmWater` int(8) DEFAULT '0' COMMENT '??????',
  `farmHarvestPlant` int(8) DEFAULT '0' COMMENT '??????',
  `dailyMission` int(8) DEFAULT '0' COMMENT '??????',
  `honorGift` int(8) DEFAULT '0' COMMENT '????????',
  `honorMission` int(8) DEFAULT '0' COMMENT '??????',
  `leagueMission` int(8) DEFAULT '0' COMMENT '??????',
  `leagueMatch` int(8) DEFAULT '0' COMMENT '??????',
  `heroBabel` int(8) DEFAULT '0' COMMENT '??????',
  `babel` int(8) DEFAULT '0' COMMENT '???????',
  `guildDonate` int(8) DEFAULT '0' COMMENT '??????',
  `guildAward` int(8) DEFAULT '0' COMMENT '??????',
  `guildPray` int(8) DEFAULT '0' COMMENT '??????',
  `heroPra` int(8) DEFAULT '0' COMMENT '??????',
  `treatAllHero` int(8) DEFAULT '0' COMMENT '??????',
  `userStrategy` int(8) DEFAULT '0' COMMENT '??????',
  `userGiveExp` int(8) DEFAULT '0' COMMENT '??????',
  `fastBuild` int(8) DEFAULT '0' COMMENT '????????',
  `studyTech` int(8) DEFAULT '0' COMMENT '??????',
  `destroyEquip` int(8) DEFAULT '0' COMMENT '??????',
  `equipStronger` int(8) DEFAULT '0' COMMENT '??????',
  `buildWallDef` int(8) DEFAULT '0' COMMENT '??????',
  `box1` int(8) DEFAULT '0' COMMENT '??1????',
  `box2` int(8) DEFAULT '0' COMMENT '??2????',
  `box3` int(8) DEFAULT '0' COMMENT '??3????',
  `box4` int(8) DEFAULT '0' COMMENT '??4????',
  `gainLeagueDefAward` timestamp NULL DEFAULT NULL COMMENT '??????????',
  `baoMin` int(2) DEFAULT '0' COMMENT '????????(0-???;1-???)',
  `liuKou` int(2) DEFAULT '0' COMMENT '????????(0-???;1-???)',
  `shuZu` int(2) DEFAULT '0' COMMENT '????????(0-???;1-???)',
  `panJun` int(2) DEFAULT '0' COMMENT '????????(0-???;1-???)',
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of dailymovement
-- ----------------------------

-- ----------------------------
-- Table structure for declarewar
-- ----------------------------
DROP TABLE IF EXISTS `declarewar`;
CREATE TABLE `declarewar` (
  `declareWarId` int(16) NOT NULL AUTO_INCREMENT,
  `fromUserId` int(16) NOT NULL,
  `toUserId` int(16) NOT NULL,
  `declareDttm` timestamp NULL DEFAULT NULL,
  `warStartDttm` timestamp NULL DEFAULT NULL,
  `finishDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`declareWarId`),
  KEY `index_DeclareWar_fromUserId` (`fromUserId`),
  KEY `index_DeclareWar_toUserId` (`toUserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of declarewar
-- ----------------------------

-- ----------------------------
-- Table structure for defplan
-- ----------------------------
DROP TABLE IF EXISTS `defplan`;
CREATE TABLE `defplan` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) DEFAULT '0',
  `heroId1` int(16) DEFAULT '0',
  `heroId2` int(16) DEFAULT '0',
  `heroId3` int(16) DEFAULT '0',
  `heroId4` int(16) DEFAULT '0',
  `heroId5` int(16) DEFAULT '0',
  `type` int(2) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of defplan
-- ----------------------------

-- ----------------------------
-- Table structure for deftroop
-- ----------------------------
DROP TABLE IF EXISTS `deftroop`;
CREATE TABLE `deftroop` (
  `defTroopId` int(16) NOT NULL AUTO_INCREMENT,
  `attackOutId` int(16) DEFAULT '0',
  `casId` int(16) DEFAULT '0',
  `fromUserId` int(16) DEFAULT '0',
  `heroId` varchar(100) DEFAULT NULL,
  `status` int(2) DEFAULT '0',
  PRIMARY KEY (`defTroopId`),
  KEY `index_DefTroop_casId` (`casId`),
  KEY `index_DefTroop_fromUserId` (`fromUserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of deftroop
-- ----------------------------

-- ----------------------------
-- Table structure for dialogset
-- ----------------------------
DROP TABLE IF EXISTS `dialogset`;
CREATE TABLE `dialogset` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `dialogId` int(16) DEFAULT '0',
  `content` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????????';

-- ----------------------------
-- Records of dialogset
-- ----------------------------

-- ----------------------------
-- Table structure for droppack
-- ----------------------------
DROP TABLE IF EXISTS `droppack`;
CREATE TABLE `droppack` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `dropPackId` int(16) DEFAULT '0',
  `packType` int(2) DEFAULT '0',
  `valueType` int(16) DEFAULT '0',
  `dropPercent` int(4) DEFAULT '0',
  `minValue` int(8) DEFAULT '0',
  `maxValue` int(8) DEFAULT '0',
  `dropItemArea` varchar(200) DEFAULT NULL,
  `weight` int(4) DEFAULT '0',
  `missionId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of droppack
-- ----------------------------

-- ----------------------------
-- Table structure for effect
-- ----------------------------
DROP TABLE IF EXISTS `effect`;
CREATE TABLE `effect` (
  `effectId` varchar(40) NOT NULL,
  `effectName` varchar(100) NOT NULL,
  `effectDesc` varchar(200) NOT NULL,
  `showFlag` int(2) DEFAULT '0',
  `equipEffectViewOrder` int(4) DEFAULT '0',
  PRIMARY KEY (`effectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of effect
-- ----------------------------

-- ----------------------------
-- Table structure for entity
-- ----------------------------
DROP TABLE IF EXISTS `entity`;
CREATE TABLE `entity` (
  `entId` int(16) NOT NULL AUTO_INCREMENT,
  `entType` varchar(20) NOT NULL,
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of entity
-- ----------------------------

-- ----------------------------
-- Table structure for entitycapacity
-- ----------------------------
DROP TABLE IF EXISTS `entitycapacity`;
CREATE TABLE `entitycapacity` (
  `entId` int(16) NOT NULL DEFAULT '0',
  `level` int(16) NOT NULL DEFAULT '0',
  `capacity` int(8) DEFAULT '0',
  `capDesc` varchar(500) DEFAULT NULL,
  `para1` int(8) DEFAULT '0',
  `para2` int(8) DEFAULT '0',
  `iconPath` varchar(40) DEFAULT NULL,
  `type1` varchar(100) DEFAULT NULL,
  `type2` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`entId`,`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of entitycapacity
-- ----------------------------

-- ----------------------------
-- Table structure for entityconsume
-- ----------------------------
DROP TABLE IF EXISTS `entityconsume`;
CREATE TABLE `entityconsume` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `entId` int(16) NOT NULL,
  `level` int(4) NOT NULL DEFAULT '0',
  `needEntId` int(16) NOT NULL DEFAULT '0',
  `needEntNum` int(8) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of entityconsume
-- ----------------------------

-- ----------------------------
-- Table structure for entitylimit
-- ----------------------------
DROP TABLE IF EXISTS `entitylimit`;
CREATE TABLE `entitylimit` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `entId` int(16) NOT NULL,
  `level` int(4) NOT NULL DEFAULT '0',
  `needEntId` int(16) NOT NULL,
  `needLevel` int(4) NOT NULL DEFAULT '0',
  `leastNum` int(8) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of entitylimit
-- ----------------------------

-- ----------------------------
-- Table structure for enumer
-- ----------------------------
DROP TABLE IF EXISTS `enumer`;
CREATE TABLE `enumer` (
  `enumId` varchar(40) NOT NULL,
  `enumGroup` varchar(40) NOT NULL,
  `enumValue` varchar(100) NOT NULL,
  `enumDesc` varchar(200) NOT NULL,
  `orderBy` int(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`enumId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of enumer
-- ----------------------------

-- ----------------------------
-- Table structure for equip
-- ----------------------------
DROP TABLE IF EXISTS `equip`;
CREATE TABLE `equip` (
  `entId` int(16) NOT NULL,
  `childType` int(2) DEFAULT '0',
  `heroLevel` int(4) DEFAULT '0',
  `heroTitle` int(4) DEFAULT '0',
  `value` varchar(500) DEFAULT NULL,
  `heroProf` int(2) DEFAULT '0',
  `suitId` int(16) DEFAULT '0',
  `holeMaxNum` int(2) DEFAULT '0',
  `currHoleNum` int(2) DEFAULT '0',
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of equip
-- ----------------------------

-- ----------------------------
-- Table structure for equipdestroy
-- ----------------------------
DROP TABLE IF EXISTS `equipdestroy`;
CREATE TABLE `equipdestroy` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `color` int(2) DEFAULT '0',
  `level` int(4) DEFAULT '0',
  `jingHuaId` int(8) DEFAULT '0',
  `num1` int(2) DEFAULT '0',
  `percent1` int(4) DEFAULT '0',
  `num2` int(2) DEFAULT '0',
  `percent2` int(4) DEFAULT '0',
  `num3` int(2) DEFAULT '0',
  `percent3` int(4) DEFAULT '0',
  `needMoney` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of equipdestroy
-- ----------------------------

-- ----------------------------
-- Table structure for equiplevelup
-- ----------------------------
DROP TABLE IF EXISTS `equiplevelup`;
CREATE TABLE `equiplevelup` (
  `level` int(2) NOT NULL DEFAULT '0',
  `jingHuaId` int(8) DEFAULT '0',
  `jingHuaNum` int(4) DEFAULT '0',
  `itemId` int(8) DEFAULT '0',
  `itemNum` int(2) DEFAULT '0',
  `addPercent` int(4) DEFAULT '0',
  `winPercent` int(4) DEFAULT '0',
  `loseLevel` int(2) DEFAULT '0',
  `addChat` int(2) DEFAULT '0',
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of equiplevelup
-- ----------------------------

-- ----------------------------
-- Table structure for equipstronger
-- ----------------------------
DROP TABLE IF EXISTS `equipstronger`;
CREATE TABLE `equipstronger` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `equipStrongerId` int(16) DEFAULT '0',
  `property` varchar(40) DEFAULT NULL,
  `value1` varchar(100) DEFAULT NULL,
  `value2` varchar(200) DEFAULT NULL,
  `itemId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `index_EquipStronger_equipStrongerId` (`equipStrongerId`),
  KEY `index_EquipStronger_property` (`property`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of equipstronger
-- ----------------------------

-- ----------------------------
-- Table structure for event
-- ----------------------------
DROP TABLE IF EXISTS `event`;
CREATE TABLE `event` (
  `eventId` varchar(100) NOT NULL,
  `actNum` int(2) DEFAULT '0',
  `sendMail` int(2) DEFAULT '0',
  `mailTitle` varchar(100) DEFAULT NULL,
  `mailContext` varchar(800) DEFAULT NULL,
  `sendChat` int(2) DEFAULT '0',
  `chatContext` varchar(200) DEFAULT NULL,
  `channel` varchar(20) DEFAULT NULL,
  `childChannel` varchar(20) DEFAULT NULL,
  `sendWin` int(2) DEFAULT '0',
  `winStartId` int(8) DEFAULT '0',
  `sendPChat` int(2) DEFAULT '0',
  `chatPContext` varchar(200) DEFAULT NULL,
  `chatPChannel` varchar(20) DEFAULT NULL,
  `chatPChildChannel` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`eventId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of event
-- ----------------------------

-- ----------------------------
-- Table structure for eventtip
-- ----------------------------
DROP TABLE IF EXISTS `eventtip`;
CREATE TABLE `eventtip` (
  `name` varchar(20) NOT NULL,
  `endTime` timestamp NULL DEFAULT NULL,
  `accMemo` varchar(80) DEFAULT NULL,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of eventtip
-- ----------------------------

-- ----------------------------
-- Table structure for eventtype
-- ----------------------------
DROP TABLE IF EXISTS `eventtype`;
CREATE TABLE `eventtype` (
  `barLv` int(16) NOT NULL DEFAULT '0',
  `eventLv` int(16) DEFAULT '0',
  `proTime` int(16) DEFAULT '0',
  `eventNum` int(16) DEFAULT '0',
  PRIMARY KEY (`barLv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of eventtype
-- ----------------------------

-- ----------------------------
-- Table structure for eventwindow
-- ----------------------------
DROP TABLE IF EXISTS `eventwindow`;
CREATE TABLE `eventwindow` (
  `winId` int(16) NOT NULL DEFAULT '0',
  `nextWinId` int(16) DEFAULT '0',
  `showType` int(2) DEFAULT '0',
  `ques` varchar(20) DEFAULT NULL,
  `ansScript` varchar(200) DEFAULT NULL,
  `imgUrl` varchar(100) DEFAULT NULL,
  `context` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`winId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of eventwindow
-- ----------------------------

-- ----------------------------
-- Table structure for examination
-- ----------------------------
DROP TABLE IF EXISTS `examination`;
CREATE TABLE `examination` (
  `code` int(16) NOT NULL,
  `subject` varchar(400) NOT NULL,
  `option1` varchar(100) NOT NULL,
  `option2` varchar(100) NOT NULL,
  `option3` varchar(100) NOT NULL,
  `answer` varchar(100) NOT NULL,
  `type` int(1) NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of examination
-- ----------------------------

-- ----------------------------
-- Table structure for examinationitem
-- ----------------------------
DROP TABLE IF EXISTS `examinationitem`;
CREATE TABLE `examinationitem` (
  `code` int(16) NOT NULL,
  `examCode` int(16) NOT NULL,
  `itemId` int(16) NOT NULL,
  `count` int(8) NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of examinationitem
-- ----------------------------

-- ----------------------------
-- Table structure for examinationtype
-- ----------------------------
DROP TABLE IF EXISTS `examinationtype`;
CREATE TABLE `examinationtype` (
  `type` int(16) NOT NULL AUTO_INCREMENT,
  `ringCount` int(4) NOT NULL DEFAULT '0',
  `actionPoint` int(8) NOT NULL DEFAULT '0',
  `minCount` int(4) DEFAULT '0',
  PRIMARY KEY (`type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of examinationtype
-- ----------------------------

-- ----------------------------
-- Table structure for fastinfo
-- ----------------------------
DROP TABLE IF EXISTS `fastinfo`;
CREATE TABLE `fastinfo` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `type1` int(2) DEFAULT '0',
  `time1` int(16) DEFAULT '0',
  `money1` int(8) DEFAULT '0',
  `type2` int(2) DEFAULT '0',
  `time2` int(16) DEFAULT '0',
  `money2` int(8) DEFAULT '0',
  `type3` int(2) DEFAULT '0',
  `time3` int(16) DEFAULT '0',
  `money3` int(8) DEFAULT '0',
  `type4` int(2) DEFAULT '0',
  `time4` int(16) DEFAULT '0',
  `money4` int(8) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????????????';

-- ----------------------------
-- Records of fastinfo
-- ----------------------------

-- ----------------------------
-- Table structure for fightdesc
-- ----------------------------
DROP TABLE IF EXISTS `fightdesc`;
CREATE TABLE `fightdesc` (
  `code` int(16) NOT NULL AUTO_INCREMENT,
  `fdesc` varchar(200) NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of fightdesc
-- ----------------------------

-- ----------------------------
-- Table structure for formation
-- ----------------------------
DROP TABLE IF EXISTS `formation`;
CREATE TABLE `formation` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `name` varchar(200) DEFAULT NULL,
  `userNumLimit` int(4) DEFAULT '0',
  `picName` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??';

-- ----------------------------
-- Records of formation
-- ----------------------------

-- ----------------------------
-- Table structure for formationpath
-- ----------------------------
DROP TABLE IF EXISTS `formationpath`;
CREATE TABLE `formationpath` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `formationId` int(16) DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  `picName` varchar(200) DEFAULT NULL,
  `pathType` int(2) DEFAULT '0',
  `pathLength` int(8) DEFAULT '0',
  `startPoint` int(16) DEFAULT '0',
  `endPoint` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of formationpath
-- ----------------------------

-- ----------------------------
-- Table structure for formationpoint
-- ----------------------------
DROP TABLE IF EXISTS `formationpoint`;
CREATE TABLE `formationpoint` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `formationId` int(16) DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  `picName` varchar(200) DEFAULT NULL,
  `pointType` int(2) DEFAULT '0',
  `isDefBase` int(2) DEFAULT '0',
  `fightType` int(2) DEFAULT '0',
  `fightUserNum` int(2) DEFAULT '0',
  `pointIntegral` int(4) DEFAULT '0',
  `wallId` int(16) DEFAULT '0',
  `defRecoverRate` double(8,2) DEFAULT '0.00',
  `attRecoverRate` double(8,2) DEFAULT '0.00',
  `defSupplyType` int(2) DEFAULT '0',
  `attSupplyType` int(2) DEFAULT '0',
  `posX` int(4) DEFAULT '0',
  `posY` int(4) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of formationpoint
-- ----------------------------

-- ----------------------------
-- Table structure for friend
-- ----------------------------
DROP TABLE IF EXISTS `friend`;
CREATE TABLE `friend` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) NOT NULL,
  `friendUserId` int(16) NOT NULL,
  `type` int(2) NOT NULL DEFAULT '0',
  `friendValue` int(4) NOT NULL DEFAULT '0',
  `addTime` timestamp NULL DEFAULT NULL,
  `appMemo` varchar(80) DEFAULT NULL,
  `accMemo` varchar(80) DEFAULT NULL,
  `isQQFriend` int(2) DEFAULT NULL,
  `friendMainCasId` int(16) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_Friend_userId` (`userId`),
  KEY `index_Friend_friendUserId` (`friendUserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of friend
-- ----------------------------

-- ----------------------------
-- Table structure for friendevalution
-- ----------------------------
DROP TABLE IF EXISTS `friendevalution`;
CREATE TABLE `friendevalution` (
  `evalutionId` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `userId` int(11) DEFAULT NULL COMMENT '??id',
  `friendUserId` int(11) DEFAULT NULL COMMENT '??id',
  `friendUserName` varchar(256) DEFAULT NULL COMMENT '???',
  `evalutionContent` varchar(1028) DEFAULT NULL COMMENT '????',
  `createTime` timestamp NULL DEFAULT NULL COMMENT '????',
  `status` int(4) DEFAULT '0' COMMENT '??(0-??;1-??)',
  PRIMARY KEY (`evalutionId`),
  KEY `index_FriendEvalution_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of friendevalution
-- ----------------------------

-- ----------------------------
-- Table structure for gamestate
-- ----------------------------
DROP TABLE IF EXISTS `gamestate`;
CREATE TABLE `gamestate` (
  `threadName` varchar(20) NOT NULL COMMENT '???',
  `startTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  `loopStartTime` timestamp NULL DEFAULT NULL COMMENT '????????',
  `loopEndTime` timestamp NULL DEFAULT NULL COMMENT '????????',
  `runNum` int(16) DEFAULT '0' COMMENT '?????',
  `intervalTime` int(16) DEFAULT '0' COMMENT '??????(????)',
  `delayNum` int(16) DEFAULT '0' COMMENT '?????',
  `recordTime` timestamp NULL DEFAULT NULL COMMENT '????????',
  PRIMARY KEY (`threadName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????';

-- ----------------------------
-- Records of gamestate
-- ----------------------------

-- ----------------------------
-- Table structure for govpotz
-- ----------------------------
DROP TABLE IF EXISTS `govpotz`;
CREATE TABLE `govpotz` (
  `govPotzId` int(16) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) DEFAULT NULL,
  `proType` int(2) DEFAULT '0',
  `addNum` double(8,2) DEFAULT '0.00',
  `addLead` int(4) DEFAULT '0',
  `merit` int(8) DEFAULT '0',
  `heroLevel` int(4) DEFAULT '0',
  `itemNum` int(2) DEFAULT '0',
  `skillId` int(16) DEFAULT '0',
  PRIMARY KEY (`govPotzId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of govpotz
-- ----------------------------

-- ----------------------------
-- Table structure for groupauthoritymapping
-- ----------------------------
DROP TABLE IF EXISTS `groupauthoritymapping`;
CREATE TABLE `groupauthoritymapping` (
  `code` int(8) NOT NULL AUTO_INCREMENT,
  `groupCode` int(4) NOT NULL,
  `authorityCode` int(4) NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of groupauthoritymapping
-- ----------------------------

-- ----------------------------
-- Table structure for guide
-- ----------------------------
DROP TABLE IF EXISTS `guide`;
CREATE TABLE `guide` (
  `guideId` int(16) NOT NULL AUTO_INCREMENT,
  `nextGuideId` int(16) DEFAULT '0',
  `quest1` varchar(20) DEFAULT NULL,
  `quest2` varchar(20) DEFAULT NULL,
  `ansScript1` varchar(200) DEFAULT NULL,
  `ansScript2` varchar(200) DEFAULT NULL,
  `stageName` varchar(40) DEFAULT NULL,
  `showType` varchar(20) DEFAULT NULL,
  `imgUrl` varchar(200) DEFAULT NULL,
  `posX` int(4) DEFAULT '0',
  `posY` int(4) DEFAULT '0',
  `point` varchar(100) DEFAULT NULL,
  `closeCondition` varchar(200) DEFAULT NULL,
  `colseGuideId` int(16) DEFAULT '0',
  `context` varchar(800) DEFAULT NULL,
  `title` varchar(40) DEFAULT NULL,
  `location` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`guideId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of guide
-- ----------------------------

-- ----------------------------
-- Table structure for guild
-- ----------------------------
DROP TABLE IF EXISTS `guild`;
CREATE TABLE `guild` (
  `guildId` int(16) NOT NULL AUTO_INCREMENT COMMENT '??Id',
  `name` varchar(40) NOT NULL COMMENT '????',
  `flag` char(2) NOT NULL COMMENT '??',
  `level` int(2) NOT NULL COMMENT '??',
  `guildDesc` varchar(1000) DEFAULT NULL COMMENT '??',
  `guildNotice` varchar(1000) DEFAULT NULL COMMENT '??',
  `honour` int(8) NOT NULL COMMENT '??',
  `capital` int(16) NOT NULL COMMENT '??',
  `state` int(2) DEFAULT '0' COMMENT '??',
  `dismissTime` datetime DEFAULT NULL COMMENT '????',
  `levelUpTime` timestamp NULL DEFAULT NULL COMMENT '????',
  `creator` varchar(40) NOT NULL DEFAULT '' COMMENT '???',
  `construction` int(16) DEFAULT '0' COMMENT '???',
  `propertyNum` int(16) DEFAULT '0' COMMENT '???',
  `picPath` varchar(100) DEFAULT NULL COMMENT '????',
  `sellProDttm` timestamp NULL DEFAULT NULL COMMENT '????????',
  `buyProDttm` timestamp NULL DEFAULT NULL COMMENT '????????',
  `sellTacticNum` int(16) DEFAULT '0' COMMENT '??????',
  `buyTacticNum` int(16) DEFAULT '0' COMMENT '??????',
  `prayTimes` int(16) DEFAULT '0' COMMENT '?????',
  `countryId` int(2) DEFAULT '0' COMMENT '????id',
  `upgradeItem` int(16) DEFAULT '0' COMMENT '????????',
  `upgradeCon` int(16) DEFAULT '0' COMMENT '????????',
  `createDttm` timestamp NULL DEFAULT NULL COMMENT '????',
  `QQGroup` varchar(20) DEFAULT NULL COMMENT 'QQ??',
  `fireNum` int(8) DEFAULT '0' COMMENT '??????',
  `dismissState` int(2) NOT NULL DEFAULT '0' COMMENT '????(0-??;1-??)',
  PRIMARY KEY (`guildId`),
  KEY `idx_name` (`name`),
  KEY `idx_flag` (`flag`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='[??] ?????????';

-- ----------------------------
-- Records of guild
-- ----------------------------

-- ----------------------------
-- Table structure for guildapplyinvite
-- ----------------------------
DROP TABLE IF EXISTS `guildapplyinvite`;
CREATE TABLE `guildapplyinvite` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `guildId` int(16) DEFAULT '0',
  `userId` int(16) DEFAULT '0',
  `title` varchar(20) DEFAULT NULL,
  `content` varchar(100) DEFAULT NULL,
  `time` timestamp NULL DEFAULT NULL,
  `type` int(2) DEFAULT '0',
  `inviteUserName` varchar(100) DEFAULT NULL,
  `accGuildId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `idx_userId` (`userId`),
  KEY `idx_guildId` (`guildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of guildapplyinvite
-- ----------------------------

-- ----------------------------
-- Table structure for guildbui
-- ----------------------------
DROP TABLE IF EXISTS `guildbui`;
CREATE TABLE `guildbui` (
  `buiId` int(16) NOT NULL DEFAULT '0',
  `buiName` varchar(100) DEFAULT NULL,
  `buiDesc` varchar(500) DEFAULT NULL,
  `maxLv` int(2) DEFAULT '0',
  `picPath` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`buiId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of guildbui
-- ----------------------------

-- ----------------------------
-- Table structure for guildbuieffect
-- ----------------------------
DROP TABLE IF EXISTS `guildbuieffect`;
CREATE TABLE `guildbuieffect` (
  `id` int(16) NOT NULL DEFAULT '0',
  `buiId` int(16) DEFAULT '0',
  `buiLv` int(2) DEFAULT '0',
  `buiName` varchar(100) DEFAULT NULL,
  `effectName` varchar(100) DEFAULT NULL,
  `effectDesc` varchar(500) DEFAULT NULL,
  `picPath` varchar(100) DEFAULT NULL,
  `guildCon` int(16) DEFAULT '0',
  `upgradeTime` int(16) DEFAULT '0',
  `upgradeHonor` int(16) DEFAULT '0',
  `relyBuiId` int(16) DEFAULT '0',
  `relyBuiLv` int(2) DEFAULT '0',
  `relyGuildLv` int(2) DEFAULT '0',
  `effectId` varchar(100) DEFAULT NULL,
  `para1` int(16) DEFAULT '0',
  `para2` int(16) DEFAULT '0',
  `para3` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of guildbuieffect
-- ----------------------------

-- ----------------------------
-- Table structure for guildbuitech
-- ----------------------------
DROP TABLE IF EXISTS `guildbuitech`;
CREATE TABLE `guildbuitech` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `guildId` int(16) DEFAULT '0',
  `buiTechId` int(16) DEFAULT '0',
  `level` int(2) DEFAULT '0',
  `type` int(2) DEFAULT '0',
  `state` int(2) DEFAULT '0',
  `endDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_GuildBuiTech_guildId` (`guildId`),
  KEY `index_GuildBuiTech_buiTechId` (`buiTechId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????,??';

-- ----------------------------
-- Records of guildbuitech
-- ----------------------------

-- ----------------------------
-- Table structure for guilddailymovement
-- ----------------------------
DROP TABLE IF EXISTS `guilddailymovement`;
CREATE TABLE `guilddailymovement` (
  `guildId` int(16) NOT NULL COMMENT '??id',
  `leagueDefCount` int(2) DEFAULT '0' COMMENT '??????',
  `resetTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  PRIMARY KEY (`guildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????????';

-- ----------------------------
-- Records of guilddailymovement
-- ----------------------------

-- ----------------------------
-- Table structure for guildevent
-- ----------------------------
DROP TABLE IF EXISTS `guildevent`;
CREATE TABLE `guildevent` (
  `eventId` int(16) NOT NULL AUTO_INCREMENT COMMENT '??ID',
  `userId` int(16) DEFAULT '0' COMMENT '??????',
  `guildId` int(8) NOT NULL DEFAULT '0' COMMENT '??Id',
  `eventContent` varchar(200) NOT NULL COMMENT '????',
  `eventType` varchar(100) NOT NULL COMMENT '????',
  `happenTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`eventId`),
  KEY `index_GuildEvent_guildId` (`guildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???????';

-- ----------------------------
-- Records of guildevent
-- ----------------------------

-- ----------------------------
-- Table structure for guildformation
-- ----------------------------
DROP TABLE IF EXISTS `guildformation`;
CREATE TABLE `guildformation` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `guildId` int(16) DEFAULT '0',
  `formationId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???????';

-- ----------------------------
-- Records of guildformation
-- ----------------------------

-- ----------------------------
-- Table structure for guildlvnum
-- ----------------------------
DROP TABLE IF EXISTS `guildlvnum`;
CREATE TABLE `guildlvnum` (
  `guildLv` int(8) NOT NULL DEFAULT '0',
  `maxLeaderNum` int(8) DEFAULT '0',
  `maxViceLeaderNum` int(8) DEFAULT '0',
  `maxGeneralNum` int(8) DEFAULT '0',
  `maxMemberNum` int(8) DEFAULT '0',
  `awardMoney` int(16) DEFAULT '0',
  `awardExperience` int(16) DEFAULT '0',
  `awardSkill` int(16) DEFAULT '0',
  `awardProperty` int(16) DEFAULT '0',
  `awardItem` int(16) DEFAULT '0',
  `upgradeTime` int(16) DEFAULT '0',
  `construction` int(16) DEFAULT '0',
  `awardHexagram` int(16) DEFAULT '0',
  `affectWoodSpeed` int(16) DEFAULT '0',
  `affectStoneSpeed` int(16) DEFAULT '0',
  `affectBronzeSpeed` int(16) DEFAULT '0',
  `affectFoodSpeed` int(16) DEFAULT '0',
  `affectMoneySpeed` int(16) DEFAULT '0',
  `affectPopSpeed` int(16) DEFAULT '0',
  `offerAward` int(16) DEFAULT '0',
  `upgradeHonor` int(16) DEFAULT '0',
  `upgradeNeedHonor` int(16) DEFAULT '0',
  PRIMARY KEY (`guildLv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????----?????';

-- ----------------------------
-- Records of guildlvnum
-- ----------------------------

-- ----------------------------
-- Table structure for guildprestige
-- ----------------------------
DROP TABLE IF EXISTS `guildprestige`;
CREATE TABLE `guildprestige` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `guildId` int(16) DEFAULT '0',
  `powerId` int(16) DEFAULT '0',
  `prestigeValue` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of guildprestige
-- ----------------------------

-- ----------------------------
-- Table structure for guildquit
-- ----------------------------
DROP TABLE IF EXISTS `guildquit`;
CREATE TABLE `guildquit` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `guildId` int(16) DEFAULT '0',
  `quitDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of guildquit
-- ----------------------------

-- ----------------------------
-- Table structure for guildrankingtmp
-- ----------------------------
DROP TABLE IF EXISTS `guildrankingtmp`;
CREATE TABLE `guildrankingtmp` (
  `rankId` int(16) NOT NULL DEFAULT '0' COMMENT 'id',
  `guildId` int(16) DEFAULT '0' COMMENT '??id',
  `level` int(4) DEFAULT '1' COMMENT '????',
  `honor` int(16) DEFAULT '100' COMMENT '????',
  `capital` int(4) DEFAULT '0' COMMENT '???',
  PRIMARY KEY (`rankId`),
  KEY `guildId` (`guildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????(???)';

-- ----------------------------
-- Records of guildrankingtmp
-- ----------------------------

-- ----------------------------
-- Table structure for guildrelation
-- ----------------------------
DROP TABLE IF EXISTS `guildrelation`;
CREATE TABLE `guildrelation` (
  `selfGuildId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `animosityGuildId` int(16) NOT NULL DEFAULT '0' COMMENT '????Id',
  `animosity` int(16) DEFAULT '0' COMMENT '???',
  `lastFlushTime` timestamp NULL DEFAULT NULL COMMENT '????????',
  PRIMARY KEY (`selfGuildId`,`animosityGuildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of guildrelation
-- ----------------------------

-- ----------------------------
-- Table structure for guildright
-- ----------------------------
DROP TABLE IF EXISTS `guildright`;
CREATE TABLE `guildright` (
  `duty` int(2) NOT NULL DEFAULT '0',
  `dutyName` varchar(20) DEFAULT NULL,
  `dismissGuild` int(2) DEFAULT '0',
  `changeLeader` int(2) DEFAULT '0',
  `castleDiplomacy` int(2) DEFAULT '0',
  `changeDuty` int(2) DEFAULT '0',
  `fireMember` int(2) DEFAULT '0',
  `changeInfoNotice` int(2) DEFAULT '0',
  `inviteMember` int(2) DEFAULT '0',
  `permitApply` int(2) DEFAULT '0',
  `viewMemberStatus` int(2) DEFAULT '0',
  `quitGuild` int(2) DEFAULT '0',
  `viewMemberList` int(2) DEFAULT '0',
  `broadcastMesssage` int(2) DEFAULT '0',
  `upgradeGuild` int(2) DEFAULT '0',
  `upgradeCastleDev` int(2) DEFAULT '0',
  `manageTrade` int(2) DEFAULT '0',
  PRIMARY KEY (`duty`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????---????\r\n           ??????????????1, ???0';

-- ----------------------------
-- Records of guildright
-- ----------------------------

-- ----------------------------
-- Table structure for guildtask
-- ----------------------------
DROP TABLE IF EXISTS `guildtask`;
CREATE TABLE `guildtask` (
  `taskId` int(16) NOT NULL AUTO_INCREMENT,
  `taskName` varchar(100) DEFAULT NULL,
  `taskDesc` varchar(500) DEFAULT NULL,
  `taskPic` varchar(100) DEFAULT NULL,
  `taskType` int(2) DEFAULT '0',
  `relyTaskId` int(16) DEFAULT '0',
  `scriptName` varchar(100) DEFAULT NULL,
  `para1` int(16) DEFAULT '0',
  `para2` int(16) DEFAULT '0',
  `para3` int(16) DEFAULT '0',
  PRIMARY KEY (`taskId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of guildtask
-- ----------------------------

-- ----------------------------
-- Table structure for guildtaskaward
-- ----------------------------
DROP TABLE IF EXISTS `guildtaskaward`;
CREATE TABLE `guildtaskaward` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `taskId` int(16) DEFAULT '0',
  `awardType` varchar(100) DEFAULT NULL,
  `awardId` int(16) DEFAULT '0',
  `awardNum` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of guildtaskaward
-- ----------------------------

-- ----------------------------
-- Table structure for guildtech
-- ----------------------------
DROP TABLE IF EXISTS `guildtech`;
CREATE TABLE `guildtech` (
  `techId` int(16) NOT NULL DEFAULT '0',
  `techName` varchar(100) DEFAULT NULL,
  `techDesc` varchar(500) DEFAULT NULL,
  `picPath` varchar(100) DEFAULT NULL,
  `maxLv` int(2) DEFAULT '0',
  PRIMARY KEY (`techId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of guildtech
-- ----------------------------

-- ----------------------------
-- Table structure for guildtecheffect
-- ----------------------------
DROP TABLE IF EXISTS `guildtecheffect`;
CREATE TABLE `guildtecheffect` (
  `id` int(16) NOT NULL DEFAULT '0',
  `techId` int(16) DEFAULT '0',
  `techLv` int(2) DEFAULT '0',
  `techName` varchar(100) DEFAULT NULL,
  `effectName` varchar(100) DEFAULT NULL,
  `effectDesc` varchar(500) DEFAULT NULL,
  `picPath` varchar(100) DEFAULT NULL,
  `guildCon` int(16) DEFAULT '0',
  `upgradeTime` int(16) DEFAULT '0',
  `upgradeHonor` int(16) DEFAULT '0',
  `relyBuiId` int(16) DEFAULT '0',
  `relyBuiLv` int(2) DEFAULT '0',
  `relyGuildLv` int(2) DEFAULT '0',
  `effectId` varchar(100) DEFAULT NULL,
  `para1` int(16) DEFAULT '0',
  `para2` int(16) DEFAULT '0',
  `para3` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of guildtecheffect
-- ----------------------------

-- ----------------------------
-- Table structure for guildtrans
-- ----------------------------
DROP TABLE IF EXISTS `guildtrans`;
CREATE TABLE `guildtrans` (
  `guildTransId` int(11) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `sellerUserId` int(11) DEFAULT NULL COMMENT '????id',
  `sellerUserName` varchar(100) DEFAULT NULL COMMENT '?????',
  `itemId` int(11) DEFAULT NULL COMMENT '??id',
  `itemNum` int(11) DEFAULT NULL COMMENT '????',
  `equipStrongerId` int(11) DEFAULT NULL COMMENT '????id',
  `price` int(11) DEFAULT NULL COMMENT '??',
  `sellTime` timestamp NULL DEFAULT NULL COMMENT '????',
  `buyerUserId` int(11) DEFAULT '-1' COMMENT '????id',
  `buyerUserName` varchar(100) DEFAULT NULL COMMENT '?????',
  `buyTime` timestamp NULL DEFAULT NULL COMMENT '????',
  `state` int(11) DEFAULT NULL COMMENT '??(0-??;1-??)',
  `itemType` int(11) DEFAULT NULL COMMENT '????',
  `guildId` int(11) DEFAULT NULL COMMENT '??id',
  PRIMARY KEY (`guildTransId`),
  KEY `index_GuildTrans_sellerUserId` (`sellerUserId`),
  KEY `index_GuildTrans_guildId` (`guildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of guildtrans
-- ----------------------------

-- ----------------------------
-- Table structure for guildwar
-- ----------------------------
DROP TABLE IF EXISTS `guildwar`;
CREATE TABLE `guildwar` (
  `attackGuildId` int(16) NOT NULL DEFAULT '0' COMMENT '?????id',
  `defendGuildId` int(16) NOT NULL DEFAULT '0' COMMENT '?????Id',
  `warFinishTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  `warColdingFinishTime` timestamp NULL DEFAULT NULL COMMENT '?????????',
  `status` int(2) DEFAULT '0' COMMENT '????(0-??;1-??;2-??)',
  PRIMARY KEY (`attackGuildId`,`defendGuildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of guildwar
-- ----------------------------

-- ----------------------------
-- Table structure for helpinfo
-- ----------------------------
DROP TABLE IF EXISTS `helpinfo`;
CREATE TABLE `helpinfo` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `menuId` int(16) DEFAULT '0',
  `preOrder` int(2) DEFAULT '0',
  `text` varchar(2000) DEFAULT NULL,
  `pic1` varchar(20) DEFAULT NULL,
  `pic2` varchar(20) DEFAULT NULL,
  `pic3` varchar(20) DEFAULT NULL,
  `title` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of helpinfo
-- ----------------------------

-- ----------------------------
-- Table structure for helpmenu
-- ----------------------------
DROP TABLE IF EXISTS `helpmenu`;
CREATE TABLE `helpmenu` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `name` varchar(40) DEFAULT NULL,
  `uri` varchar(200) DEFAULT NULL,
  `fatherId` int(16) DEFAULT '0',
  `keyWord` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of helpmenu
-- ----------------------------

-- ----------------------------
-- Table structure for hero
-- ----------------------------
DROP TABLE IF EXISTS `hero`;
CREATE TABLE `hero` (
  `heroId` int(16) NOT NULL AUTO_INCREMENT,
  `casId` int(16) NOT NULL,
  `userId` int(16) DEFAULT '0',
  `name` varchar(16) DEFAULT '0',
  `picPath` varchar(100) DEFAULT NULL COMMENT '??',
  `exp` int(16) DEFAULT '0' COMMENT '??',
  `level` int(4) DEFAULT '0',
  `maxLevel` int(4) DEFAULT NULL,
  `gen` int(2) DEFAULT '0' COMMENT '??',
  `chartr` int(4) DEFAULT '0' COMMENT '??',
  `careerId` int(2) DEFAULT '0' COMMENT '??',
  `merit` int(8) DEFAULT '0' COMMENT '??',
  `upCount` double(8,2) DEFAULT '0.00' COMMENT '????',
  `hp` int(8) DEFAULT '0' COMMENT '??',
  `atk` int(8) DEFAULT '0' COMMENT '??',
  `def` int(8) DEFAULT '0' COMMENT '??',
  `agl` int(8) DEFAULT '0' COMMENT '??',
  `fen1` int(4) DEFAULT NULL,
  `fen2` int(4) DEFAULT NULL,
  `fen3` int(4) DEFAULT NULL,
  `fen4` int(4) DEFAULT NULL,
  `lead` int(8) DEFAULT '0' COMMENT '??',
  `changeCount` int(2) DEFAULT '0' COMMENT '????',
  `title` int(4) DEFAULT '0' COMMENT '??',
  `spel1` int(8) DEFAULT NULL,
  `equ1` int(16) DEFAULT NULL,
  `equ2` int(16) DEFAULT NULL,
  `equ3` int(16) DEFAULT NULL,
  `equ4` int(16) DEFAULT NULL,
  `equ5` int(16) DEFAULT '0',
  `status` int(2) DEFAULT '0' COMMENT '??:??,???,????',
  `hpStatus` int(2) DEFAULT '0' COMMENT '??????',
  `actionStatus` int(2) DEFAULT NULL,
  `armyEntId` int(16) DEFAULT '0',
  `armyNum` int(8) DEFAULT '0',
  `changeTime` timestamp NULL DEFAULT NULL,
  `govPotzTime` timestamp NULL DEFAULT NULL,
  `govPotzId` int(16) DEFAULT '0',
  `heroMissionId` int(16) DEFAULT '0',
  `combatSkillId` int(16) DEFAULT '0',
  `fightSkillId` int(16) DEFAULT '0',
  `govPotzEndTime` timestamp NULL DEFAULT NULL,
  `atkXuanYan` varchar(40) DEFAULT NULL,
  `practiceStartDttm` timestamp NULL DEFAULT NULL,
  `defXuanYan` varchar(40) DEFAULT NULL,
  `practiceEndDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`heroId`),
  KEY `index_Hero_userId` (`userId`),
  KEY `index_Hero_casId` (`casId`),
  KEY `index_Hero_actionStatus` (`actionStatus`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of hero
-- ----------------------------

-- ----------------------------
-- Table structure for herobattle
-- ----------------------------
DROP TABLE IF EXISTS `herobattle`;
CREATE TABLE `herobattle` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `name` varchar(20) DEFAULT NULL,
  `descInfo` varchar(200) DEFAULT NULL,
  `recomLevel` int(4) DEFAULT '0',
  `openCon` varchar(200) DEFAULT NULL,
  `place` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???(??) ';

-- ----------------------------
-- Records of herobattle
-- ----------------------------

-- ----------------------------
-- Table structure for herobattlerecord
-- ----------------------------
DROP TABLE IF EXISTS `herobattlerecord`;
CREATE TABLE `herobattlerecord` (
  `userId` int(16) NOT NULL DEFAULT '0',
  `battleId` int(16) NOT NULL DEFAULT '0',
  `stageId` int(16) NOT NULL DEFAULT '0',
  `itemNum` int(2) DEFAULT '0',
  PRIMARY KEY (`userId`,`battleId`,`stageId`),
  KEY `index_HeroBattleRecord_userId` (`userId`),
  KEY `index_HeroBattleRecord_stageId` (`stageId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????????';

-- ----------------------------
-- Records of herobattlerecord
-- ----------------------------

-- ----------------------------
-- Table structure for herobattlescore
-- ----------------------------
DROP TABLE IF EXISTS `herobattlescore`;
CREATE TABLE `herobattlescore` (
  `score` int(2) NOT NULL DEFAULT '0',
  `scoreDesc` varchar(20) DEFAULT NULL,
  `itemNum` int(2) DEFAULT '0',
  PRIMARY KEY (`score`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????';

-- ----------------------------
-- Records of herobattlescore
-- ----------------------------

-- ----------------------------
-- Table structure for herobattlestage
-- ----------------------------
DROP TABLE IF EXISTS `herobattlestage`;
CREATE TABLE `herobattlestage` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `battleId` int(16) DEFAULT '0',
  `stageOrder` int(4) DEFAULT '0',
  `name` varchar(20) DEFAULT NULL,
  `isBoss` int(2) DEFAULT '0',
  `stageDesc` varchar(200) DEFAULT NULL,
  `hard` int(2) DEFAULT '0',
  `pveCastleId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???(??) ';

-- ----------------------------
-- Records of herobattlestage
-- ----------------------------

-- ----------------------------
-- Table structure for heroeffect
-- ----------------------------
DROP TABLE IF EXISTS `heroeffect`;
CREATE TABLE `heroeffect` (
  `playerEffId` int(16) NOT NULL AUTO_INCREMENT,
  `heroId` int(16) DEFAULT '0' COMMENT '??id',
  `effectId` varchar(40) DEFAULT NULL COMMENT '??id',
  `type` varchar(40) DEFAULT NULL COMMENT '????(?????...)',
  `itemEffectId` int(8) DEFAULT '0' COMMENT '????id',
  `absValue` int(8) DEFAULT '0' COMMENT '?????',
  `perValue` int(8) DEFAULT '0' COMMENT '?????',
  `showFlag` int(2) DEFAULT '0' COMMENT '(?????)',
  `expireDttm` timestamp NULL DEFAULT NULL COMMENT '??????',
  PRIMARY KEY (`playerEffId`),
  KEY `index_HeroEffect_heroId` (`heroId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of heroeffect
-- ----------------------------

-- ----------------------------
-- Table structure for heroexp
-- ----------------------------
DROP TABLE IF EXISTS `heroexp`;
CREATE TABLE `heroexp` (
  `level` int(4) NOT NULL DEFAULT '0',
  `exp` int(16) DEFAULT '0',
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of heroexp
-- ----------------------------

-- ----------------------------
-- Table structure for herolead
-- ----------------------------
DROP TABLE IF EXISTS `herolead`;
CREATE TABLE `herolead` (
  `level` int(16) NOT NULL AUTO_INCREMENT,
  `lead` int(4) DEFAULT '0',
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of herolead
-- ----------------------------

-- ----------------------------
-- Table structure for heroname
-- ----------------------------
DROP TABLE IF EXISTS `heroname`;
CREATE TABLE `heroname` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `sex` int(2) DEFAULT '0',
  `first` varchar(800) DEFAULT NULL,
  `second` varchar(800) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of heroname
-- ----------------------------

-- ----------------------------
-- Table structure for heropic
-- ----------------------------
DROP TABLE IF EXISTS `heropic`;
CREATE TABLE `heropic` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `gen` int(2) DEFAULT '0',
  `iconPath` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of heropic
-- ----------------------------

-- ----------------------------
-- Table structure for heroquality
-- ----------------------------
DROP TABLE IF EXISTS `heroquality`;
CREATE TABLE `heroquality` (
  `q1` int(4) DEFAULT '0',
  `q2` int(4) DEFAULT NULL,
  `q3` int(4) DEFAULT NULL,
  `q4` int(4) DEFAULT NULL,
  `q5` int(4) DEFAULT NULL,
  `q6` int(4) DEFAULT NULL,
  `q7` int(4) DEFAULT NULL,
  `q8` int(4) DEFAULT NULL,
  `q9` int(4) DEFAULT NULL,
  `q10` int(4) DEFAULT NULL,
  `q11` int(4) DEFAULT NULL,
  `q12` int(4) DEFAULT NULL,
  `q13` int(4) DEFAULT NULL,
  `q14` int(4) DEFAULT NULL,
  `q15` int(4) DEFAULT NULL,
  `q16` int(4) DEFAULT NULL,
  `q17` int(4) DEFAULT NULL,
  `q18` int(4) DEFAULT NULL,
  `q19` int(4) DEFAULT NULL,
  `q20` int(4) DEFAULT NULL,
  `id` int(16) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of heroquality
-- ----------------------------

-- ----------------------------
-- Table structure for heroquapercent
-- ----------------------------
DROP TABLE IF EXISTS `heroquapercent`;
CREATE TABLE `heroquapercent` (
  `pc1` int(4) DEFAULT NULL,
  `pc2` int(4) DEFAULT NULL,
  `pc3` int(4) DEFAULT NULL,
  `pc4` int(4) DEFAULT '0',
  `career` int(2) DEFAULT '0',
  `id` int(16) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of heroquapercent
-- ----------------------------

-- ----------------------------
-- Table structure for heroskill
-- ----------------------------
DROP TABLE IF EXISTS `heroskill`;
CREATE TABLE `heroskill` (
  `heroId` int(16) NOT NULL DEFAULT '0',
  `userId` int(16) DEFAULT '0',
  `combatSkill_0` int(16) DEFAULT '0',
  `combatSkill_1` int(16) DEFAULT '0',
  `combatSkill_2` int(16) DEFAULT '0',
  `combatSkill_3` int(16) DEFAULT '0',
  `fightSkill_0` int(16) DEFAULT '0',
  `fightSkill_1` int(16) DEFAULT '0',
  `fightSkill_2` int(16) DEFAULT '0',
  `fightSkill_3` int(16) DEFAULT '0',
  PRIMARY KEY (`heroId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of heroskill
-- ----------------------------

-- ----------------------------
-- Table structure for herostronger
-- ----------------------------
DROP TABLE IF EXISTS `herostronger`;
CREATE TABLE `herostronger` (
  `id` int(16) NOT NULL DEFAULT '0',
  `q1` int(16) DEFAULT '0',
  `q2` int(16) DEFAULT NULL,
  `q3` int(16) DEFAULT NULL,
  `q4` int(16) DEFAULT NULL,
  `q5` int(16) DEFAULT NULL,
  `q6` int(16) DEFAULT NULL,
  `q7` int(16) DEFAULT NULL,
  `q8` int(16) DEFAULT NULL,
  `q9` int(16) DEFAULT NULL,
  `q10` int(16) DEFAULT NULL,
  `q11` int(16) DEFAULT NULL,
  `q12` int(16) DEFAULT NULL,
  `q13` int(16) DEFAULT NULL,
  `q14` int(16) DEFAULT NULL,
  `q15` int(16) DEFAULT NULL,
  `q16` int(16) DEFAULT NULL,
  `q17` int(16) DEFAULT NULL,
  `q18` int(16) DEFAULT NULL,
  `q19` int(16) DEFAULT NULL,
  `q20` int(16) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of herostronger
-- ----------------------------

-- ----------------------------
-- Table structure for heroxuanyan
-- ----------------------------
DROP TABLE IF EXISTS `heroxuanyan`;
CREATE TABLE `heroxuanyan` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `chartr` int(2) DEFAULT '0',
  `content` varchar(40) DEFAULT NULL,
  `type` int(2) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of heroxuanyan
-- ----------------------------

-- ----------------------------
-- Table structure for hospital
-- ----------------------------
DROP TABLE IF EXISTS `hospital`;
CREATE TABLE `hospital` (
  `casId` int(16) NOT NULL DEFAULT '0',
  `hosPoint` int(8) DEFAULT '0',
  `lastUpdateTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of hospital
-- ----------------------------

-- ----------------------------
-- Table structure for impact
-- ----------------------------
DROP TABLE IF EXISTS `impact`;
CREATE TABLE `impact` (
  `impactId` varchar(40) NOT NULL,
  `quotiety` int(4) NOT NULL DEFAULT '0',
  `para1` varchar(20) NOT NULL,
  `para1Num` int(4) NOT NULL DEFAULT '0',
  `para2` varchar(20) NOT NULL,
  `para2Num` int(4) NOT NULL DEFAULT '0',
  `sendRate` int(4) NOT NULL DEFAULT '0',
  `target` varchar(20) DEFAULT NULL,
  `effectRoundNum` int(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`impactId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of impact
-- ----------------------------

-- ----------------------------
-- Table structure for iplimit
-- ----------------------------
DROP TABLE IF EXISTS `iplimit`;
CREATE TABLE `iplimit` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `ipAddr` varchar(40) DEFAULT NULL,
  `createDttm` timestamp NULL DEFAULT NULL,
  `memo` varchar(100) DEFAULT NULL,
  `isWhiteGroup` int(2) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of iplimit
-- ----------------------------

-- ----------------------------
-- Table structure for item
-- ----------------------------
DROP TABLE IF EXISTS `item`;
CREATE TABLE `item` (
  `entId` int(16) NOT NULL,
  `itemName` varchar(40) NOT NULL,
  `itemDesc` varchar(500) DEFAULT NULL,
  `iconPath` varchar(100) DEFAULT NULL,
  `type` int(2) DEFAULT '0',
  `effectId` int(16) NOT NULL,
  `sumAble` int(2) DEFAULT '0',
  `bandAble` int(2) DEFAULT NULL,
  `throwAble` int(2) DEFAULT NULL,
  `useAble` int(2) DEFAULT NULL,
  `useMaxNum` int(4) DEFAULT '0',
  `childType` int(255) DEFAULT '0',
  `level` int(2) DEFAULT '0',
  `color` int(2) DEFAULT '0',
  `sellPrice` int(8) DEFAULT '0',
  `buyPrice` int(8) DEFAULT '0',
  `buyHonor` int(8) DEFAULT '0',
  `buyGuild` int(8) DEFAULT '0',
  `fameType` int(2) DEFAULT '0',
  `fameValue` int(8) DEFAULT '0',
  `dropAble` int(2) DEFAULT '0',
  `instDesc` varchar(100) DEFAULT NULL,
  `instCur` int(8) DEFAULT '0',
  `instMax` int(8) DEFAULT '0',
  `missionId` int(16) DEFAULT '0',
  `time` int(4) DEFAULT '0',
  `gemValue` varchar(200) DEFAULT NULL,
  `gemNeedNum` int(4) DEFAULT '0',
  `nextGemLevelEntId` int(16) DEFAULT '0',
  `userHasMaxNum` int(4) DEFAULT '0',
  `useContent` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of item
-- ----------------------------

-- ----------------------------
-- Table structure for itemeffect
-- ----------------------------
DROP TABLE IF EXISTS `itemeffect`;
CREATE TABLE `itemeffect` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `beanName` varchar(100) DEFAULT NULL,
  `para1` int(16) DEFAULT '0',
  `para2` int(16) DEFAULT '0',
  `para3` int(16) DEFAULT '0',
  `effectName` varchar(20) DEFAULT NULL,
  `effectDesc` varchar(200) DEFAULT NULL,
  `effectLevel` int(2) DEFAULT '0',
  `iconPath` varchar(40) DEFAULT NULL,
  `effectType` varchar(20) DEFAULT NULL,
  `childType` varchar(255) DEFAULT NULL,
  `buffName` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of itemeffect
-- ----------------------------

-- ----------------------------
-- Table structure for jobcontext
-- ----------------------------
DROP TABLE IF EXISTS `jobcontext`;
CREATE TABLE `jobcontext` (
  `jobId` int(16) NOT NULL AUTO_INCREMENT,
  `jobGroupName` varchar(20) NOT NULL,
  `jobIdInGroup` varchar(40) NOT NULL,
  `jobExcuteTime` bigint(16) DEFAULT '0',
  `className` varchar(200) DEFAULT NULL,
  `jobPara` blob,
  `jobType` int(2) DEFAULT '0',
  `dispatch` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`jobId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of jobcontext
-- ----------------------------

-- ----------------------------
-- Table structure for lastarmyoutrecord
-- ----------------------------
DROP TABLE IF EXISTS `lastarmyoutrecord`;
CREATE TABLE `lastarmyoutrecord` (
  `casId` int(16) NOT NULL,
  `posX` int(4) DEFAULT '0',
  `posY` int(4) DEFAULT '0',
  `type` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of lastarmyoutrecord
-- ----------------------------

-- ----------------------------
-- Table structure for league
-- ----------------------------
DROP TABLE IF EXISTS `league`;
CREATE TABLE `league` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `level` int(2) DEFAULT '0' COMMENT '??',
  `name` varchar(100) DEFAULT NULL COMMENT '????',
  `userNum` int(8) DEFAULT '0' COMMENT '?????',
  `upgradeNum` int(2) DEFAULT '0' COMMENT '????',
  `downgradeNum` int(2) DEFAULT '0' COMMENT '????',
  `upLevel` int(4) DEFAULT '0' COMMENT '?????(0??????)',
  `downLevel` int(16) DEFAULT '0' COMMENT '?????(0??????)',
  `awardWinMeritorious` int(16) DEFAULT '0' COMMENT '??????(?????)',
  `awardLoseMeritorious` int(16) DEFAULT '0' COMMENT '??????????????)',
  `npcTroopId` int(16) DEFAULT '0' COMMENT 'npc(?????)',
  `hallUserNum` int(4) DEFAULT '0' COMMENT '??????',
  `maxJoinTime` int(2) DEFAULT '0' COMMENT '?????????',
  `startTimeExp` varchar(20) DEFAULT NULL COMMENT '??????',
  `endTimeExp` varchar(20) DEFAULT NULL COMMENT '??????',
  `turnTime` int(4) DEFAULT '0' COMMENT '?????',
  `openStatus` int(2) DEFAULT '0' COMMENT '????(0-??,1-??????,??id=-1????',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of league
-- ----------------------------

-- ----------------------------
-- Table structure for leagueaward
-- ----------------------------
DROP TABLE IF EXISTS `leagueaward`;
CREATE TABLE `leagueaward` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `level` int(2) DEFAULT '0',
  `awardItemId` int(16) DEFAULT '0',
  `awardItemNum` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of leagueaward
-- ----------------------------

-- ----------------------------
-- Table structure for leaguedefnpc
-- ----------------------------
DROP TABLE IF EXISTS `leaguedefnpc`;
CREATE TABLE `leaguedefnpc` (
  `level` int(2) NOT NULL COMMENT '??',
  `npcName` varchar(40) DEFAULT NULL COMMENT 'npc??',
  `pveCastleIdList` varchar(800) DEFAULT NULL COMMENT 'npc??(id;id)',
  `maxPower` varchar(20) DEFAULT NULL COMMENT '?????',
  `guildAward` int(8) DEFAULT '0' COMMENT '????(????)',
  `userAward` int(8) DEFAULT '0' COMMENT '????(??)',
  `maxCastleNum` int(2) DEFAULT '0' COMMENT '?????????',
  `consumeGuildPoint` int(8) DEFAULT '0' COMMENT '????????',
  `para1` int(16) DEFAULT '0' COMMENT '??1',
  `para2` int(16) DEFAULT '0' COMMENT '??2',
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????npc';

-- ----------------------------
-- Records of leaguedefnpc
-- ----------------------------

-- ----------------------------
-- Table structure for leaguedefrecord
-- ----------------------------
DROP TABLE IF EXISTS `leaguedefrecord`;
CREATE TABLE `leaguedefrecord` (
  `guildId` int(16) NOT NULL COMMENT '??id',
  `atkLevel` int(2) DEFAULT '0' COMMENT '???????',
  `winMaxLevel` int(2) DEFAULT '0' COMMENT '??????',
  `result` int(2) DEFAULT '0' COMMENT '??(1-??;2-??;0-???;3-???)',
  `endTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  `castleDefInfo` varchar(500) DEFAULT NULL COMMENT '??????',
  PRIMARY KEY (`guildId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????npc??????';

-- ----------------------------
-- Records of leaguedefrecord
-- ----------------------------

-- ----------------------------
-- Table structure for leaguemission
-- ----------------------------
DROP TABLE IF EXISTS `leaguemission`;
CREATE TABLE `leaguemission` (
  `leagueMissionId` int(16) NOT NULL AUTO_INCREMENT,
  `leagueId` int(16) DEFAULT '0',
  `missionId` int(16) DEFAULT '0',
  `completeNum` int(8) DEFAULT '0',
  `status` int(2) DEFAULT '0',
  `limitTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`leagueMissionId`),
  KEY `idx_leagueId` (`leagueId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of leaguemission
-- ----------------------------

-- ----------------------------
-- Table structure for leaguerankingtmp
-- ----------------------------
DROP TABLE IF EXISTS `leaguerankingtmp`;
CREATE TABLE `leaguerankingtmp` (
  `rankId` int(16) NOT NULL DEFAULT '0' COMMENT 'id',
  `userId` int(16) DEFAULT '0' COMMENT '??id',
  `leagueLv` int(4) DEFAULT '1' COMMENT '??????',
  `seasonIntegral` int(16) DEFAULT '0' COMMENT '????',
  `seansonWinNum` int(4) DEFAULT '0' COMMENT '?????',
  `seasonLoseNum` int(4) DEFAULT '0' COMMENT '?????',
  PRIMARY KEY (`rankId`),
  KEY `userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????(???)';

-- ----------------------------
-- Records of leaguerankingtmp
-- ----------------------------

-- ----------------------------
-- Table structure for leagueseason
-- ----------------------------
DROP TABLE IF EXISTS `leagueseason`;
CREATE TABLE `leagueseason` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `leagueLevel` int(2) DEFAULT '0',
  `seasonNum` int(4) DEFAULT '1',
  `roundNum` int(2) DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of leagueseason
-- ----------------------------

-- ----------------------------
-- Table structure for leagueuser
-- ----------------------------
DROP TABLE IF EXISTS `leagueuser`;
CREATE TABLE `leagueuser` (
  `userId` int(16) NOT NULL DEFAULT '0',
  `userName` varchar(100) DEFAULT NULL,
  `leagueLevel` int(2) DEFAULT '0',
  `isNew` int(2) DEFAULT '0',
  `roundWinNum` int(8) DEFAULT '0',
  `roundLoseNum` int(2) DEFAULT '0',
  `seasonWinNum` int(8) DEFAULT '0',
  `seasonLoseNum` int(2) DEFAULT '0',
  `roundIntegral` int(16) DEFAULT '0',
  `seasonIntegral` int(16) DEFAULT '0',
  `roundMerit` int(16) DEFAULT '0',
  `oldLevel` int(2) DEFAULT '0',
  `joinTime` int(2) DEFAULT '0',
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???????';

-- ----------------------------
-- Records of leagueuser
-- ----------------------------

-- ----------------------------
-- Table structure for loginactivity
-- ----------------------------
DROP TABLE IF EXISTS `loginactivity`;
CREATE TABLE `loginactivity` (
  `id` int(16) NOT NULL DEFAULT '0',
  `title` varchar(40) DEFAULT NULL,
  `startDttm` varchar(40) DEFAULT NULL,
  `endDttm` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of loginactivity
-- ----------------------------

-- ----------------------------
-- Table structure for loginnews
-- ----------------------------
DROP TABLE IF EXISTS `loginnews`;
CREATE TABLE `loginnews` (
  `id` int(16) NOT NULL DEFAULT '0',
  `title` varchar(40) DEFAULT NULL,
  `url` varchar(100) DEFAULT NULL,
  `date` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of loginnews
-- ----------------------------

-- ----------------------------
-- Table structure for logintime
-- ----------------------------
DROP TABLE IF EXISTS `logintime`;
CREATE TABLE `logintime` (
  `userId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `missionId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `missionStartTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  `missionEndTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  `lastLoginDate` date DEFAULT NULL COMMENT '??????',
  `continueLoginDays` int(16) DEFAULT '0' COMMENT '??????',
  `lastLoginTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  `contiueLoginTimes` int(16) DEFAULT '0' COMMENT '??????',
  `lastMaxLoginDays` int(8) DEFAULT '0' COMMENT '???????????',
  PRIMARY KEY (`userId`,`missionId`),
  KEY `missionId` (`missionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????(???????????)';

-- ----------------------------
-- Records of logintime
-- ----------------------------

-- ----------------------------
-- Table structure for luckevent
-- ----------------------------
DROP TABLE IF EXISTS `luckevent`;
CREATE TABLE `luckevent` (
  `eventId` int(16) NOT NULL AUTO_INCREMENT,
  `eventLv` int(16) DEFAULT '0',
  `luckType` int(16) DEFAULT '0',
  `eventName` varchar(100) DEFAULT NULL,
  `eventDesc` varchar(200) DEFAULT NULL,
  `firstOption` varchar(100) DEFAULT NULL,
  `secondOption` varchar(100) DEFAULT NULL,
  `personId` int(16) DEFAULT '0',
  `needResId` int(16) DEFAULT '0',
  `needResNum` int(16) DEFAULT '0',
  `awardFood` int(16) DEFAULT '0',
  `awardWood` int(16) DEFAULT '0',
  `awardBronze` int(16) DEFAULT '0',
  `awardMoney` int(16) DEFAULT '0',
  `awardPop` int(16) DEFAULT '0',
  `awardNewArmy` int(16) DEFAULT '0',
  `awardHonor` int(16) DEFAULT '0',
  `awardExpPoint` int(16) DEFAULT '0',
  `awardProPoint` int(16) DEFAULT '0',
  `awardCasRange` int(16) DEFAULT '0',
  `awardStone` int(16) DEFAULT '0',
  `picPath` varchar(100) DEFAULT NULL,
  `resBad` varchar(200) DEFAULT NULL,
  `resNormal` varchar(200) DEFAULT NULL,
  `resGood` varchar(200) DEFAULT NULL,
  `resBetter` varchar(200) DEFAULT NULL,
  `rebuild` int(4) DEFAULT '0' COMMENT '????????????(0,?;1,?)',
  PRIMARY KEY (`eventId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of luckevent
-- ----------------------------

-- ----------------------------
-- Table structure for lucktip
-- ----------------------------
DROP TABLE IF EXISTS `lucktip`;
CREATE TABLE `lucktip` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `luckType` int(2) DEFAULT '0',
  `luckValue` int(2) DEFAULT '0',
  `luckTip` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of lucktip
-- ----------------------------

-- ----------------------------
-- Table structure for lucktype
-- ----------------------------
DROP TABLE IF EXISTS `lucktype`;
CREATE TABLE `lucktype` (
  `typeId` int(16) NOT NULL DEFAULT '0',
  `typeName` varchar(100) DEFAULT NULL,
  `typeDesc` varchar(200) DEFAULT NULL,
  `typeIcon` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`typeId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????--????,??..';

-- ----------------------------
-- Records of lucktype
-- ----------------------------

-- ----------------------------
-- Table structure for lvparalimit
-- ----------------------------
DROP TABLE IF EXISTS `lvparalimit`;
CREATE TABLE `lvparalimit` (
  `actionPoint` int(16) DEFAULT '0',
  `rankId` int(16) NOT NULL AUTO_INCREMENT,
  `rankName` varchar(40) DEFAULT NULL,
  `expPointLimit` int(16) DEFAULT '0',
  `skillPointLimit` int(16) DEFAULT '0',
  `militaryLimit` int(16) DEFAULT '0',
  `intelLimit` int(16) DEFAULT '0',
  `polityLimit` int(16) DEFAULT '0',
  `honor` int(16) DEFAULT '0',
  `rankDesc` varchar(500) DEFAULT NULL,
  `armyNumLimit` int(16) DEFAULT '0',
  `achieve` int(16) DEFAULT '0',
  `honorMissNum` int(8) DEFAULT '0',
  `tacticLimit` int(16) DEFAULT '0',
  `resource` int(16) DEFAULT '0',
  `money` int(16) DEFAULT '0',
  `skillPoint` int(16) DEFAULT '0',
  `propertyPoint` int(16) DEFAULT '0',
  `picPath` varchar(100) DEFAULT NULL,
  `cityNum` int(2) DEFAULT '0',
  `tipTitle` varchar(40) DEFAULT NULL,
  `tipContent` varchar(500) DEFAULT NULL,
  `hexHonor` int(16) DEFAULT '0',
  `minHonorMissionLevel` int(2) DEFAULT '0',
  `maxHonorMissionLevel` int(2) DEFAULT '0',
  `pvpNum` int(4) DEFAULT '0' COMMENT 'pvp????',
  PRIMARY KEY (`rankId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????????';

-- ----------------------------
-- Records of lvparalimit
-- ----------------------------

-- ----------------------------
-- Table structure for lvvalue
-- ----------------------------
DROP TABLE IF EXISTS `lvvalue`;
CREATE TABLE `lvvalue` (
  `lvId` int(16) NOT NULL AUTO_INCREMENT,
  `prestigeLv` varchar(40) DEFAULT NULL,
  `donate` int(2) DEFAULT '0',
  `mission` int(2) DEFAULT '0',
  `activity` int(2) DEFAULT '0',
  `soul` int(2) DEFAULT '0',
  `minValue` int(16) DEFAULT '0',
  `maxValue` int(16) DEFAULT '0',
  `itemId` int(8) DEFAULT '0',
  `num` int(4) DEFAULT '0',
  `res` int(8) DEFAULT '0',
  `money` int(8) DEFAULT '0',
  `userExp` int(8) DEFAULT '0',
  `userExpPool` int(8) DEFAULT '0',
  PRIMARY KEY (`lvId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='???????????';

-- ----------------------------
-- Records of lvvalue
-- ----------------------------

-- ----------------------------
-- Table structure for mapcell
-- ----------------------------
DROP TABLE IF EXISTS `mapcell`;
CREATE TABLE `mapcell` (
  `posX` int(4) NOT NULL DEFAULT '0',
  `posY` int(4) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  `rangeLv` int(4) DEFAULT '0',
  `type` varchar(20) NOT NULL,
  `casId` int(16) NOT NULL,
  `userId` int(16) NOT NULL,
  `orderNo` int(2) DEFAULT '0',
  `flag` varchar(4) DEFAULT '0',
  `branchCasTypeList` varchar(40) DEFAULT NULL,
  `stateId` int(4) DEFAULT '0',
  `leagueAttack` int(4) DEFAULT '0',
  `otherflag` varchar(20) DEFAULT NULL,
  `qqType` varchar(20) DEFAULT NULL COMMENT 'qq????',
  `qqEndTime` timestamp NULL DEFAULT NULL COMMENT '????',
  PRIMARY KEY (`posX`,`posY`),
  KEY `index_MapCell_casId` (`casId`),
  KEY `index_MapCell_stateId` (`stateId`),
  KEY `index_MapCell_orderNo` (`orderNo`),
  KEY `userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of mapcell
-- ----------------------------

-- ----------------------------
-- Table structure for mapoutarmyview
-- ----------------------------
DROP TABLE IF EXISTS `mapoutarmyview`;
CREATE TABLE `mapoutarmyview` (
  `attackOutId` int(16) NOT NULL DEFAULT '0',
  `outPosX` int(4) DEFAULT '0',
  `outPosY` int(4) DEFAULT '0',
  `outCasId` int(8) DEFAULT '0',
  `outUserId` int(8) DEFAULT '0',
  `outGuildId` int(8) DEFAULT '0',
  `toPosX` int(4) DEFAULT '0',
  `toPosY` int(4) DEFAULT '0',
  `toCasId` int(8) DEFAULT '0',
  `toUserId` int(8) DEFAULT '0',
  `toGuildId` int(8) DEFAULT '0',
  `outTime` timestamp NULL DEFAULT NULL,
  `arriveTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`attackOutId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of mapoutarmyview
-- ----------------------------

-- ----------------------------
-- Table structure for marketlvnum
-- ----------------------------
DROP TABLE IF EXISTS `marketlvnum`;
CREATE TABLE `marketlvnum` (
  `marketLv` int(8) NOT NULL DEFAULT '0',
  `speed` int(8) DEFAULT '0',
  `exportNum` int(8) DEFAULT '0',
  `importNum` int(8) DEFAULT '0',
  `loadQuantity` int(8) DEFAULT '0',
  `maxDistance` int(8) DEFAULT '0',
  `constructCost` int(8) DEFAULT '0',
  PRIMARY KEY (`marketLv`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????????----????';

-- ----------------------------
-- Records of marketlvnum
-- ----------------------------

-- ----------------------------
-- Table structure for markettrans
-- ----------------------------
DROP TABLE IF EXISTS `markettrans`;
CREATE TABLE `markettrans` (
  `transId` int(16) NOT NULL AUTO_INCREMENT,
  `transType` int(2) DEFAULT '0' COMMENT '????:0:???? 1:??-???? 11:?????????',
  `sellUserId` int(16) NOT NULL,
  `buyUserId` int(16) DEFAULT '0',
  `casId` int(16) NOT NULL COMMENT '?????Id',
  `entityId` int(16) NOT NULL COMMENT '????id',
  `count` int(4) NOT NULL DEFAULT '0' COMMENT '????',
  `singlePrice` int(8) NOT NULL DEFAULT '0' COMMENT '????',
  `leagueId` int(16) NOT NULL COMMENT '????:???????????0',
  `sellStus` int(2) NOT NULL DEFAULT '0' COMMENT '????? 0 :???? 1:??? 3:?????????',
  `buyStus` int(2) NOT NULL DEFAULT '0' COMMENT '?????               2:??? 3:?????????',
  `entityType` int(4) DEFAULT '0',
  `equipStrongerId` int(16) DEFAULT '0',
  `dexUserId` int(16) DEFAULT '0',
  `overDueTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`transId`),
  KEY `index_MarketTrans_entityId` (`entityId`),
  KEY `index_MarketTrans_sellUserId` (`sellUserId`),
  KEY `index_MarketTrans_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of markettrans
-- ----------------------------

-- ----------------------------
-- Table structure for message
-- ----------------------------
DROP TABLE IF EXISTS `message`;
CREATE TABLE `message` (
  `messageId` int(16) NOT NULL AUTO_INCREMENT,
  `sendUserId` int(16) NOT NULL,
  `receiveUserId` int(16) NOT NULL,
  `sendDttm` timestamp NULL DEFAULT NULL,
  `readDttm` timestamp NULL DEFAULT NULL,
  `comment` varchar(4000) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `messageSendType` int(2) DEFAULT '0',
  `messageReceiveType` int(2) DEFAULT '0',
  `messageType` int(2) DEFAULT '0',
  `childType` int(2) DEFAULT '0',
  `map` blob,
  `appendixFlag` int(2) DEFAULT '0',
  `entityId` int(16) DEFAULT '0' COMMENT '??id',
  `itemNum` int(16) DEFAULT '0' COMMENT '????',
  `equipStrongerId` int(16) DEFAULT '0' COMMENT '??id',
  PRIMARY KEY (`messageId`),
  KEY `index_Message_receiveUserId` (`receiveUserId`),
  KEY `message_type` (`messageType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of message
-- ----------------------------

-- ----------------------------
-- Table structure for message_0
-- ----------------------------
DROP TABLE IF EXISTS `message_0`;
CREATE TABLE `message_0` (
  `messageId` int(16) NOT NULL AUTO_INCREMENT,
  `sendUserId` int(16) NOT NULL,
  `receiveUserId` int(16) NOT NULL,
  `sendDttm` timestamp NULL DEFAULT NULL,
  `readDttm` timestamp NULL DEFAULT NULL,
  `comment` varchar(4000) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `messageSendType` int(2) DEFAULT '0',
  `messageReceiveType` int(2) DEFAULT '0',
  `messageType` int(2) DEFAULT '0',
  `childType` int(2) DEFAULT '0',
  `map` blob,
  `appendixFlag` int(2) DEFAULT '0',
  `entityId` int(16) DEFAULT '0' COMMENT '??id',
  `itemNum` int(16) DEFAULT '0' COMMENT '????',
  `equipStrongerId` int(16) DEFAULT '0' COMMENT '??id',
  PRIMARY KEY (`messageId`),
  KEY `index_Message_receiveUserId` (`receiveUserId`),
  KEY `message_type` (`messageType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of message_0
-- ----------------------------

-- ----------------------------
-- Table structure for message_1
-- ----------------------------
DROP TABLE IF EXISTS `message_1`;
CREATE TABLE `message_1` (
  `messageId` int(16) NOT NULL AUTO_INCREMENT,
  `sendUserId` int(16) NOT NULL,
  `receiveUserId` int(16) NOT NULL,
  `sendDttm` timestamp NULL DEFAULT NULL,
  `readDttm` timestamp NULL DEFAULT NULL,
  `comment` varchar(4000) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `messageSendType` int(2) DEFAULT '0',
  `messageReceiveType` int(2) DEFAULT '0',
  `messageType` int(2) DEFAULT '0',
  `childType` int(2) DEFAULT '0',
  `map` blob,
  `appendixFlag` int(2) DEFAULT '0',
  `entityId` int(16) DEFAULT '0' COMMENT '??id',
  `itemNum` int(16) DEFAULT '0' COMMENT '????',
  `equipStrongerId` int(16) DEFAULT '0' COMMENT '??id',
  PRIMARY KEY (`messageId`),
  KEY `index_Message_receiveUserId` (`receiveUserId`),
  KEY `message_type` (`messageType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of message_1
-- ----------------------------

-- ----------------------------
-- Table structure for message_2
-- ----------------------------
DROP TABLE IF EXISTS `message_2`;
CREATE TABLE `message_2` (
  `messageId` int(16) NOT NULL AUTO_INCREMENT,
  `sendUserId` int(16) NOT NULL,
  `receiveUserId` int(16) NOT NULL,
  `sendDttm` timestamp NULL DEFAULT NULL,
  `readDttm` timestamp NULL DEFAULT NULL,
  `comment` varchar(4000) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `messageSendType` int(2) DEFAULT '0',
  `messageReceiveType` int(2) DEFAULT '0',
  `messageType` int(2) DEFAULT '0',
  `childType` int(2) DEFAULT '0',
  `map` blob,
  `appendixFlag` int(2) DEFAULT '0',
  `entityId` int(16) DEFAULT '0' COMMENT '??id',
  `itemNum` int(16) DEFAULT '0' COMMENT '????',
  `equipStrongerId` int(16) DEFAULT '0' COMMENT '??id',
  PRIMARY KEY (`messageId`),
  KEY `index_Message_receiveUserId` (`receiveUserId`),
  KEY `message_type` (`messageType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of message_2
-- ----------------------------

-- ----------------------------
-- Table structure for message_3
-- ----------------------------
DROP TABLE IF EXISTS `message_3`;
CREATE TABLE `message_3` (
  `messageId` int(16) NOT NULL AUTO_INCREMENT,
  `sendUserId` int(16) NOT NULL,
  `receiveUserId` int(16) NOT NULL,
  `sendDttm` timestamp NULL DEFAULT NULL,
  `readDttm` timestamp NULL DEFAULT NULL,
  `comment` varchar(4000) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `messageSendType` int(2) DEFAULT '0',
  `messageReceiveType` int(2) DEFAULT '0',
  `messageType` int(2) DEFAULT '0',
  `childType` int(2) DEFAULT '0',
  `map` blob,
  `appendixFlag` int(2) DEFAULT '0',
  `entityId` int(16) DEFAULT '0' COMMENT '??id',
  `itemNum` int(16) DEFAULT '0' COMMENT '????',
  `equipStrongerId` int(16) DEFAULT '0' COMMENT '??id',
  PRIMARY KEY (`messageId`),
  KEY `index_Message_receiveUserId` (`receiveUserId`),
  KEY `message_type` (`messageType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of message_3
-- ----------------------------

-- ----------------------------
-- Table structure for message_4
-- ----------------------------
DROP TABLE IF EXISTS `message_4`;
CREATE TABLE `message_4` (
  `messageId` int(16) NOT NULL AUTO_INCREMENT,
  `sendUserId` int(16) NOT NULL,
  `receiveUserId` int(16) NOT NULL,
  `sendDttm` timestamp NULL DEFAULT NULL,
  `readDttm` timestamp NULL DEFAULT NULL,
  `comment` varchar(4000) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `messageSendType` int(2) DEFAULT '0',
  `messageReceiveType` int(2) DEFAULT '0',
  `messageType` int(2) DEFAULT '0',
  `childType` int(2) DEFAULT '0',
  `map` blob,
  `appendixFlag` int(2) DEFAULT '0',
  `entityId` int(16) DEFAULT '0' COMMENT '??id',
  `itemNum` int(16) DEFAULT '0' COMMENT '????',
  `equipStrongerId` int(16) DEFAULT '0' COMMENT '??id',
  PRIMARY KEY (`messageId`),
  KEY `index_Message_receiveUserId` (`receiveUserId`),
  KEY `message_type` (`messageType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of message_4
-- ----------------------------

-- ----------------------------
-- Table structure for mission
-- ----------------------------
DROP TABLE IF EXISTS `mission`;
CREATE TABLE `mission` (
  `missionId` int(16) NOT NULL AUTO_INCREMENT,
  `missionType` varchar(40) DEFAULT NULL,
  `missionName` varchar(100) DEFAULT NULL,
  `missionDesc` varchar(500) DEFAULT NULL,
  `missionCompleteDesc1` varchar(200) DEFAULT NULL,
  `missionCompleteDesc2` varchar(200) DEFAULT NULL,
  `missionCompleteDesc3` varchar(200) DEFAULT NULL,
  `missionCompleteDesc4` varchar(200) DEFAULT NULL,
  `missionCompleteDesc5` varchar(200) DEFAULT NULL,
  `missionHardLevel` int(2) DEFAULT '0',
  `missionLevel` int(2) DEFAULT '0',
  `completeCondition` int(2) DEFAULT '0',
  `serviceName` varchar(40) DEFAULT NULL,
  `parentMissionId` int(16) DEFAULT '0',
  `isDelItem` int(2) DEFAULT '0',
  `limitCount` int(4) DEFAULT '0',
  `missioncompleteId1` int(16) DEFAULT '0',
  `missioncompleteId2` int(16) DEFAULT '0',
  `missioncompleteId3` int(16) DEFAULT '0',
  `missioncompleteId4` int(16) DEFAULT '0',
  `missioncompleteId5` int(16) DEFAULT '0',
  `money` int(8) DEFAULT '0',
  `gold` int(2) DEFAULT '0',
  `resource` int(8) DEFAULT '0',
  `armyId` int(16) DEFAULT '0',
  `armyNum` int(4) DEFAULT '0',
  `itemGiveType` int(2) DEFAULT '0',
  `itemId1` int(16) DEFAULT '0',
  `itemNum1` int(2) DEFAULT '0',
  `itemId2` int(16) DEFAULT '0',
  `itemNum2` int(2) DEFAULT '0',
  `itemId3` int(16) DEFAULT '0',
  `itemNum3` int(2) DEFAULT '0',
  `itemId4` int(16) DEFAULT '0',
  `itemNum4` int(2) DEFAULT '0',
  `value1` int(8) DEFAULT '0',
  `value2` int(8) DEFAULT '0',
  `valueType1` varchar(40) DEFAULT NULL,
  `missionGuidDesc` varchar(500) DEFAULT NULL,
  `pop` int(8) DEFAULT '0',
  `rangeValue` int(8) DEFAULT '0',
  `pic1` varchar(20) DEFAULT NULL,
  `pic2` varchar(20) DEFAULT NULL,
  `pic3` varchar(20) DEFAULT NULL,
  `pic4` varchar(20) DEFAULT NULL,
  `missionChildType` varchar(20) DEFAULT NULL,
  `valueType2` varchar(40) DEFAULT NULL,
  `award` varchar(100) DEFAULT NULL,
  `value3` int(8) DEFAULT '0',
  `value4` int(8) DEFAULT '0',
  PRIMARY KEY (`missionId`),
  KEY `idx_parentMissionId` (`parentMissionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of mission
-- ----------------------------

-- ----------------------------
-- Table structure for missionlimit
-- ----------------------------
DROP TABLE IF EXISTS `missionlimit`;
CREATE TABLE `missionlimit` (
  `missioncompleteId` int(16) NOT NULL AUTO_INCREMENT,
  `entId` int(16) DEFAULT '0',
  `entNum` int(8) DEFAULT '0',
  `octType` varchar(20) DEFAULT NULL,
  `entType` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`missioncompleteId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of missionlimit
-- ----------------------------

-- ----------------------------
-- Table structure for multiplereward
-- ----------------------------
DROP TABLE IF EXISTS `multiplereward`;
CREATE TABLE `multiplereward` (
  `code` int(16) NOT NULL AUTO_INCREMENT,
  `name` varchar(40) NOT NULL,
  `startDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `endDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `multiple` int(8) NOT NULL,
  `type` int(1) NOT NULL,
  `createDate` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`code`),
  KEY `idx_endDate` (`endDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of multiplereward
-- ----------------------------

-- ----------------------------
-- Table structure for newreward
-- ----------------------------
DROP TABLE IF EXISTS `newreward`;
CREATE TABLE `newreward` (
  `achieveId` int(16) NOT NULL DEFAULT '0' COMMENT '??ID',
  `rewardType` int(2) NOT NULL DEFAULT '0' COMMENT '????(1-????)',
  `entId` int(16) NOT NULL DEFAULT '0' COMMENT '????id',
  `num` int(8) DEFAULT '0' COMMENT '????',
  `remark` varchar(100) DEFAULT NULL COMMENT '????',
  PRIMARY KEY (`achieveId`,`rewardType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????(???)';

-- ----------------------------
-- Records of newreward
-- ----------------------------

-- ----------------------------
-- Table structure for newrewardrecord
-- ----------------------------
DROP TABLE IF EXISTS `newrewardrecord`;
CREATE TABLE `newrewardrecord` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `friendId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `achieveId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `rewardType` int(2) NOT NULL DEFAULT '0' COMMENT '????',
  `finishTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  `isGet` int(2) NOT NULL DEFAULT '0' COMMENT '???? 0-???,1-???'',\r\n            ???? 0-???,1-???',
  `getTime` timestamp NULL DEFAULT NULL COMMENT '????',
  `remark` varchar(100) DEFAULT NULL COMMENT '????',
  PRIMARY KEY (`id`),
  KEY `index_NewRewardRecord_userId` (`userId`),
  KEY `index_NewRewardRecord_friendId` (`friendId`),
  KEY `index_NewRewardRecord_rewardType` (`rewardType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????';

-- ----------------------------
-- Records of newrewardrecord
-- ----------------------------

-- ----------------------------
-- Table structure for npcattacker
-- ----------------------------
DROP TABLE IF EXISTS `npcattacker`;
CREATE TABLE `npcattacker` (
  `id` int(16) NOT NULL DEFAULT '0',
  `onwer` int(2) DEFAULT '0',
  `winMin` int(4) DEFAULT '0',
  `winMax` int(4) DEFAULT '0',
  `pveCastleId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of npcattacker
-- ----------------------------

-- ----------------------------
-- Table structure for npcsellitem
-- ----------------------------
DROP TABLE IF EXISTS `npcsellitem`;
CREATE TABLE `npcsellitem` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `entId` int(16) DEFAULT '0',
  `ironLevel` int(2) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of npcsellitem
-- ----------------------------

-- ----------------------------
-- Table structure for onlinetimeaward
-- ----------------------------
DROP TABLE IF EXISTS `onlinetimeaward`;
CREATE TABLE `onlinetimeaward` (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '??ID',
  `onlineTime` int(11) DEFAULT NULL,
  `awardDesc` varchar(256) DEFAULT NULL,
  `fetchDesc` varchar(256) DEFAULT NULL,
  `packId` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of onlinetimeaward
-- ----------------------------

-- ----------------------------
-- Table structure for operator
-- ----------------------------
DROP TABLE IF EXISTS `operator`;
CREATE TABLE `operator` (
  `code` int(8) NOT NULL AUTO_INCREMENT,
  `username` varchar(60) NOT NULL,
  `password` varchar(30) NOT NULL,
  `compellation` varchar(60) NOT NULL,
  `gender` char(1) DEFAULT NULL,
  `email` varchar(60) DEFAULT NULL,
  `position` varchar(120) DEFAULT NULL,
  `carrierId` varchar(8) DEFAULT NULL,
  `groupCode` int(4) DEFAULT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of operator
-- ----------------------------

-- ----------------------------
-- Table structure for partner
-- ----------------------------
DROP TABLE IF EXISTS `partner`;
CREATE TABLE `partner` (
  `partnerId` int(16) NOT NULL AUTO_INCREMENT,
  `partnerName` varchar(50) NOT NULL,
  `validateType` varchar(20) NOT NULL,
  `validateURL` varchar(200) DEFAULT NULL,
  `errorURL` varchar(200) DEFAULT NULL,
  `manufURL` varchar(200) DEFAULT NULL,
  `manufDesc` varchar(100) DEFAULT NULL,
  `manufTroth` varchar(100) DEFAULT NULL,
  `cashKey` varchar(100) DEFAULT NULL,
  `forumURL` varchar(250) DEFAULT NULL,
  `kefuURL` varchar(250) DEFAULT NULL,
  `creditURL` varchar(300) DEFAULT NULL,
  `cashType` int(4) NOT NULL,
  `flipType` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`partnerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of partner
-- ----------------------------

-- ----------------------------
-- Table structure for party
-- ----------------------------
DROP TABLE IF EXISTS `party`;
CREATE TABLE `party` (
  `entId` int(16) NOT NULL,
  `partyName` varchar(100) DEFAULT NULL,
  `iconPath` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of party
-- ----------------------------

-- ----------------------------
-- Table structure for person
-- ----------------------------
DROP TABLE IF EXISTS `person`;
CREATE TABLE `person` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `rangeValue` int(16) DEFAULT '0',
  `name` varchar(40) DEFAULT NULL,
  `model` varchar(40) DEFAULT NULL,
  `picIcon` varchar(40) DEFAULT NULL,
  `dialogId` int(16) DEFAULT '0',
  `roadPath` varchar(40) DEFAULT NULL,
  `walkMode` int(4) DEFAULT '0',
  `walkSpeed` int(16) DEFAULT '0',
  `scriptName` varchar(40) DEFAULT NULL,
  `appearMode` int(4) DEFAULT '0',
  `appearDelay` int(8) DEFAULT '0',
  `filmNum` int(8) DEFAULT '0',
  `para1` int(8) DEFAULT '0',
  `para2` int(8) DEFAULT '0',
  `para3` int(8) DEFAULT '0',
  `timeStr` varchar(40) DEFAULT NULL,
  `disappearRange` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of person
-- ----------------------------

-- ----------------------------
-- Table structure for playereffect
-- ----------------------------
DROP TABLE IF EXISTS `playereffect`;
CREATE TABLE `playereffect` (
  `playerEffId` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `casId` int(16) DEFAULT NULL,
  `heroId` int(16) DEFAULT '0',
  `effectId` varchar(40) DEFAULT NULL,
  `type` varchar(20) DEFAULT NULL,
  `itemEffectId` int(16) DEFAULT '0',
  `absValue` int(4) DEFAULT '0',
  `perValue` int(4) DEFAULT '0',
  `expireDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`playerEffId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of playereffect
-- ----------------------------

-- ----------------------------
-- Table structure for praylvnum
-- ----------------------------
DROP TABLE IF EXISTS `praylvnum`;
CREATE TABLE `praylvnum` (
  `level` int(2) NOT NULL DEFAULT '0',
  `prayTimes` int(8) DEFAULT '0',
  `pointOfGuild` int(16) DEFAULT '0',
  `picPath` varchar(100) DEFAULT NULL,
  `boxId` int(16) DEFAULT '0',
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????';

-- ----------------------------
-- Records of praylvnum
-- ----------------------------

-- ----------------------------
-- Table structure for prefixeffect
-- ----------------------------
DROP TABLE IF EXISTS `prefixeffect`;
CREATE TABLE `prefixeffect` (
  `prefixId` varchar(20) NOT NULL,
  `prefixColor` int(2) DEFAULT '0',
  `prefixDesc` varchar(200) DEFAULT NULL,
  `prefixName` varchar(40) DEFAULT NULL,
  `prefixValue` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`prefixId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of prefixeffect
-- ----------------------------

-- ----------------------------
-- Table structure for prestige
-- ----------------------------
DROP TABLE IF EXISTS `prestige`;
CREATE TABLE `prestige` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `powerId` int(2) DEFAULT '0',
  `prestigeValue` int(16) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `index_Prestige_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of prestige
-- ----------------------------

-- ----------------------------
-- Table structure for prestigedonate
-- ----------------------------
DROP TABLE IF EXISTS `prestigedonate`;
CREATE TABLE `prestigedonate` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `powerId` int(2) DEFAULT '0',
  `donateName` varchar(40) DEFAULT NULL,
  `donateDesc` varchar(200) DEFAULT NULL,
  `itemId` int(16) DEFAULT '0',
  `itemNum` int(16) DEFAULT '0',
  `prestigeValue` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????---?????';

-- ----------------------------
-- Records of prestigedonate
-- ----------------------------

-- ----------------------------
-- Table structure for prestigepower
-- ----------------------------
DROP TABLE IF EXISTS `prestigepower`;
CREATE TABLE `prestigepower` (
  `powerId` int(16) NOT NULL AUTO_INCREMENT,
  `powerName` varchar(40) DEFAULT NULL,
  `powerDesc` varchar(500) DEFAULT NULL,
  `powerType` int(2) DEFAULT '0',
  PRIMARY KEY (`powerId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????---?????';

-- ----------------------------
-- Records of prestigepower
-- ----------------------------

-- ----------------------------
-- Table structure for prestigeshop
-- ----------------------------
DROP TABLE IF EXISTS `prestigeshop`;
CREATE TABLE `prestigeshop` (
  `shopId` int(16) NOT NULL DEFAULT '0',
  `itemId` int(16) DEFAULT '0',
  `countryId` int(2) DEFAULT '0',
  `pricePrestige` int(8) DEFAULT '0',
  `needPrestige` int(8) DEFAULT '0',
  PRIMARY KEY (`shopId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of prestigeshop
-- ----------------------------

-- ----------------------------
-- Table structure for province
-- ----------------------------
DROP TABLE IF EXISTS `province`;
CREATE TABLE `province` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `countryId` int(16) DEFAULT '0',
  `level` int(2) DEFAULT '0',
  `name` varchar(200) DEFAULT NULL,
  `needLevel` int(2) DEFAULT '0',
  `needItem` int(8) DEFAULT '0',
  `needPrestige` int(8) DEFAULT '0',
  `occuGuildId` int(16) DEFAULT '0',
  `occuDate` timestamp NULL DEFAULT NULL,
  `occuDayNum` int(4) DEFAULT '0',
  `defaUserNum` int(4) DEFAULT '0',
  `defEffect` int(4) DEFAULT '0',
  `incEffect` int(4) DEFAULT '0',
  `maxEffect` int(4) DEFAULT '0',
  `defFormationId` int(4) DEFAULT '0' COMMENT '???????',
  `defTroopId` int(16) DEFAULT '0' COMMENT '???NPC',
  `addHonour` int(8) DEFAULT '0',
  `awardType` int(2) DEFAULT '0' COMMENT '????(1-???;2-????;3-???????;4-????????;5-??????)',
  `awardNum` int(8) DEFAULT '0' COMMENT '????',
  `awardOffer` int(8) DEFAULT '0' COMMENT '???????????',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??';

-- ----------------------------
-- Records of province
-- ----------------------------

-- ----------------------------
-- Table structure for provinceapplica
-- ----------------------------
DROP TABLE IF EXISTS `provinceapplica`;
CREATE TABLE `provinceapplica` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `provinceId` int(16) DEFAULT '0',
  `guildId` int(16) DEFAULT '0',
  `itemNum` int(8) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of provinceapplica
-- ----------------------------

-- ----------------------------
-- Table structure for pvecastle
-- ----------------------------
DROP TABLE IF EXISTS `pvecastle`;
CREATE TABLE `pvecastle` (
  `pveCastleId` int(16) NOT NULL DEFAULT '0',
  `castleLevel` int(4) DEFAULT '0',
  `pveCastleType` int(2) DEFAULT '0',
  `troop1` int(16) DEFAULT '0',
  `troop2` int(16) DEFAULT '0',
  `troop3` int(16) DEFAULT '0',
  `pveWallId` int(16) DEFAULT '0',
  `own` int(2) DEFAULT '0',
  `npcName` varchar(40) DEFAULT NULL,
  `npcDesc` varchar(200) DEFAULT NULL,
  `iconPath` varchar(40) DEFAULT NULL,
  `combatItemPackId` int(16) DEFAULT '0',
  `combatExpPackId` int(16) DEFAULT '0',
  `fightItemPackId` int(16) DEFAULT '0',
  `fightExpPackId` int(16) DEFAULT '0',
  `freshTime` int(4) DEFAULT '0',
  `merit` int(8) DEFAULT '0',
  `combatPower` varchar(20) DEFAULT NULL,
  `fightPower` varchar(20) DEFAULT NULL,
  `lingWuLevel` int(2) DEFAULT '0',
  `percent` int(8) DEFAULT '0',
  `rangeValue` int(8) DEFAULT '0',
  `castleType` int(4) DEFAULT '0',
  PRIMARY KEY (`pveCastleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of pvecastle
-- ----------------------------

-- ----------------------------
-- Table structure for pvehero
-- ----------------------------
DROP TABLE IF EXISTS `pvehero`;
CREATE TABLE `pvehero` (
  `heroId` int(16) NOT NULL DEFAULT '0',
  `upCount` double(8,2) DEFAULT '0.00',
  `careerId` int(2) DEFAULT '0',
  `level` int(4) DEFAULT '0',
  `chartr` int(2) DEFAULT '0',
  `combatSkillId` int(16) DEFAULT '0',
  `fightSkillId` int(16) DEFAULT '0',
  `gen` int(2) DEFAULT '0',
  `type` int(2) NOT NULL DEFAULT '0',
  `armyEntId` int(16) DEFAULT '0',
  `armyNum` int(8) DEFAULT '0',
  `name` varchar(20) DEFAULT NULL,
  `iconPath` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`heroId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of pvehero
-- ----------------------------

-- ----------------------------
-- Table structure for pvemapcell
-- ----------------------------
DROP TABLE IF EXISTS `pvemapcell`;
CREATE TABLE `pvemapcell` (
  `mapCellId` int(16) NOT NULL AUTO_INCREMENT,
  `posX` int(4) DEFAULT '0',
  `posY` int(4) DEFAULT '0',
  `countryId` int(2) DEFAULT '0',
  `cityId` int(4) DEFAULT '0',
  `pveCastleId` int(16) DEFAULT '0',
  `reFreshTime` int(8) DEFAULT '0',
  `type` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`mapCellId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of pvemapcell
-- ----------------------------

-- ----------------------------
-- Table structure for pvetroop
-- ----------------------------
DROP TABLE IF EXISTS `pvetroop`;
CREATE TABLE `pvetroop` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `troopId` int(16) DEFAULT '0',
  `pveHeroId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `index_PveTroop_troopId` (`troopId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of pvetroop
-- ----------------------------

-- ----------------------------
-- Table structure for pveuserwinrecord
-- ----------------------------
DROP TABLE IF EXISTS `pveuserwinrecord`;
CREATE TABLE `pveuserwinrecord` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `pveCastleId` int(2) DEFAULT '0',
  `combatFlag` int(2) DEFAULT '0',
  `fightFlag` int(2) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `index_PveUserWinRecord_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of pveuserwinrecord
-- ----------------------------

-- ----------------------------
-- Table structure for pvewall
-- ----------------------------
DROP TABLE IF EXISTS `pvewall`;
CREATE TABLE `pvewall` (
  `pveWallId` int(16) NOT NULL AUTO_INCREMENT,
  `wallHp` int(8) DEFAULT '0',
  `wallDef` int(8) DEFAULT '0',
  `nuJian` int(8) DEFAULT '0',
  `gunMu` int(8) DEFAULT '0',
  `leiShi` int(8) DEFAULT '0',
  `juMa` int(8) DEFAULT '0',
  `xianJing` int(8) DEFAULT '0',
  PRIMARY KEY (`pveWallId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of pvewall
-- ----------------------------

-- ----------------------------
-- Table structure for qqfriend
-- ----------------------------
DROP TABLE IF EXISTS `qqfriend`;
CREATE TABLE `qqfriend` (
  `accId` bigint(16) NOT NULL DEFAULT '0' COMMENT '??QQ?',
  `friendPara` blob COMMENT '??????',
  `curDate` varchar(25) DEFAULT NULL COMMENT '???????',
  PRIMARY KEY (`accId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='QQ????';

-- ----------------------------
-- Records of qqfriend
-- ----------------------------

-- ----------------------------
-- Table structure for qqshop
-- ----------------------------
DROP TABLE IF EXISTS `qqshop`;
CREATE TABLE `qqshop` (
  `shopId` int(16) NOT NULL AUTO_INCREMENT,
  `itemId` int(16) DEFAULT '0' COMMENT '??id',
  `tagId` int(2) DEFAULT '6' COMMENT '??',
  `shopType` int(2) DEFAULT '3' COMMENT '????( 1-?? 2-?? 3-??)',
  `singleNum` int(4) DEFAULT '0' COMMENT '??????',
  `singlePrice` int(8) DEFAULT '0' COMMENT '??',
  `isPayPointCanBuy` int(2) DEFAULT '1' COMMENT '???????? 1:?? 0 :??',
  `pricePercent` int(4) DEFAULT '100' COMMENT '???',
  `startTime` timestamp NULL DEFAULT NULL COMMENT '????',
  `endTime` timestamp NULL DEFAULT NULL COMMENT '????',
  `sortId` int(8) DEFAULT '0' COMMENT '??, ?????',
  `itemDesc` varchar(800) DEFAULT NULL COMMENT '????',
  PRIMARY KEY (`shopId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='QQ??';

-- ----------------------------
-- Records of qqshop
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_blob_triggers
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_blob_triggers`;
CREATE TABLE `qrtz_blob_triggers` (
  `TRIGGER_NAME` varchar(80) NOT NULL,
  `TRIGGER_GROUP` varchar(80) NOT NULL,
  `BLOB_DATA` blob,
  PRIMARY KEY (`TRIGGER_NAME`,`TRIGGER_GROUP`),
  KEY `TRIGGER_NAME` (`TRIGGER_NAME`,`TRIGGER_GROUP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_blob_triggers
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_calendars
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_calendars`;
CREATE TABLE `qrtz_calendars` (
  `CALENDAR_NAME` varchar(80) NOT NULL,
  `CALENDAR` blob NOT NULL,
  PRIMARY KEY (`CALENDAR_NAME`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_calendars
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_cron_triggers
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_cron_triggers`;
CREATE TABLE `qrtz_cron_triggers` (
  `TRIGGER_NAME` varchar(80) NOT NULL,
  `TRIGGER_GROUP` varchar(80) NOT NULL,
  `CRON_EXPRESSION` varchar(80) NOT NULL,
  `TIME_ZONE_ID` varchar(80) DEFAULT NULL,
  PRIMARY KEY (`TRIGGER_NAME`,`TRIGGER_GROUP`),
  KEY `TRIGGER_NAME` (`TRIGGER_NAME`,`TRIGGER_GROUP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_cron_triggers
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_fired_triggers
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_fired_triggers`;
CREATE TABLE `qrtz_fired_triggers` (
  `ENTRY_ID` varchar(95) NOT NULL,
  `TRIGGER_NAME` varchar(80) NOT NULL,
  `TRIGGER_GROUP` varchar(80) NOT NULL,
  `IS_VOLATILE` varchar(1) NOT NULL,
  `INSTANCE_NAME` varchar(80) NOT NULL,
  `FIRED_TIME` bigint(13) NOT NULL,
  `PRIORITY` int(11) NOT NULL,
  `STATE` varchar(16) NOT NULL,
  `JOB_NAME` varchar(80) DEFAULT NULL,
  `JOB_GROUP` varchar(80) DEFAULT NULL,
  `IS_STATEFUL` varchar(1) DEFAULT NULL,
  `REQUESTS_RECOVERY` varchar(1) DEFAULT NULL,
  PRIMARY KEY (`ENTRY_ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_fired_triggers
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_job_details
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_job_details`;
CREATE TABLE `qrtz_job_details` (
  `JOB_NAME` varchar(80) NOT NULL,
  `JOB_GROUP` varchar(80) NOT NULL,
  `DESCRIPTION` varchar(120) DEFAULT NULL,
  `JOB_CLASS_NAME` varchar(128) NOT NULL,
  `IS_DURABLE` varchar(1) NOT NULL,
  `IS_VOLATILE` varchar(1) NOT NULL,
  `IS_STATEFUL` varchar(1) NOT NULL,
  `REQUESTS_RECOVERY` varchar(1) NOT NULL,
  `JOB_DATA` blob,
  PRIMARY KEY (`JOB_NAME`,`JOB_GROUP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_job_details
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_job_listeners
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_job_listeners`;
CREATE TABLE `qrtz_job_listeners` (
  `JOB_NAME` varchar(80) NOT NULL,
  `JOB_GROUP` varchar(80) NOT NULL,
  `JOB_LISTENER` varchar(80) NOT NULL,
  PRIMARY KEY (`JOB_NAME`,`JOB_GROUP`,`JOB_LISTENER`),
  KEY `JOB_NAME` (`JOB_NAME`,`JOB_GROUP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_job_listeners
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_locks
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_locks`;
CREATE TABLE `qrtz_locks` (
  `LOCK_NAME` varchar(40) NOT NULL,
  PRIMARY KEY (`LOCK_NAME`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_locks
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_paused_trigger_grps
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_paused_trigger_grps`;
CREATE TABLE `qrtz_paused_trigger_grps` (
  `TRIGGER_GROUP` varchar(80) NOT NULL,
  PRIMARY KEY (`TRIGGER_GROUP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_paused_trigger_grps
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_scheduler_state
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_scheduler_state`;
CREATE TABLE `qrtz_scheduler_state` (
  `INSTANCE_NAME` varchar(80) NOT NULL,
  `LAST_CHECKIN_TIME` bigint(13) NOT NULL,
  `CHECKIN_INTERVAL` bigint(13) NOT NULL,
  PRIMARY KEY (`INSTANCE_NAME`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_scheduler_state
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_simple_triggers
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_simple_triggers`;
CREATE TABLE `qrtz_simple_triggers` (
  `TRIGGER_NAME` varchar(80) NOT NULL,
  `TRIGGER_GROUP` varchar(80) NOT NULL,
  `REPEAT_COUNT` bigint(7) NOT NULL,
  `REPEAT_INTERVAL` bigint(12) NOT NULL,
  `TIMES_TRIGGERED` bigint(7) NOT NULL,
  PRIMARY KEY (`TRIGGER_NAME`,`TRIGGER_GROUP`),
  KEY `TRIGGER_NAME` (`TRIGGER_NAME`,`TRIGGER_GROUP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_simple_triggers
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_triggers
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_triggers`;
CREATE TABLE `qrtz_triggers` (
  `TRIGGER_NAME` varchar(80) NOT NULL,
  `TRIGGER_GROUP` varchar(80) NOT NULL,
  `JOB_NAME` varchar(80) NOT NULL,
  `JOB_GROUP` varchar(80) NOT NULL,
  `IS_VOLATILE` varchar(1) NOT NULL,
  `DESCRIPTION` varchar(120) DEFAULT NULL,
  `NEXT_FIRE_TIME` bigint(13) DEFAULT NULL,
  `PREV_FIRE_TIME` bigint(13) DEFAULT NULL,
  `PRIORITY` int(11) DEFAULT NULL,
  `TRIGGER_STATE` varchar(16) NOT NULL,
  `TRIGGER_TYPE` varchar(8) NOT NULL,
  `START_TIME` bigint(13) NOT NULL,
  `END_TIME` bigint(13) DEFAULT NULL,
  `CALENDAR_NAME` varchar(80) DEFAULT NULL,
  `MISFIRE_INSTR` smallint(2) DEFAULT NULL,
  `JOB_DATA` blob,
  PRIMARY KEY (`TRIGGER_NAME`,`TRIGGER_GROUP`),
  KEY `JOB_NAME` (`JOB_NAME`,`JOB_GROUP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_triggers
-- ----------------------------

-- ----------------------------
-- Table structure for qrtz_trigger_listeners
-- ----------------------------
DROP TABLE IF EXISTS `qrtz_trigger_listeners`;
CREATE TABLE `qrtz_trigger_listeners` (
  `TRIGGER_NAME` varchar(80) NOT NULL,
  `TRIGGER_GROUP` varchar(80) NOT NULL,
  `TRIGGER_LISTENER` varchar(80) NOT NULL,
  PRIMARY KEY (`TRIGGER_NAME`,`TRIGGER_GROUP`,`TRIGGER_LISTENER`),
  KEY `TRIGGER_NAME` (`TRIGGER_NAME`,`TRIGGER_GROUP`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of qrtz_trigger_listeners
-- ----------------------------

-- ----------------------------
-- Table structure for relivelimit
-- ----------------------------
DROP TABLE IF EXISTS `relivelimit`;
CREATE TABLE `relivelimit` (
  `reliveNum` int(16) NOT NULL DEFAULT '0',
  `reliveRequie` int(4) DEFAULT '0',
  `maxUpCount` int(4) DEFAULT '0',
  PRIMARY KEY (`reliveNum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of relivelimit
-- ----------------------------

-- ----------------------------
-- Table structure for resource
-- ----------------------------
DROP TABLE IF EXISTS `resource`;
CREATE TABLE `resource` (
  `entId` int(16) NOT NULL,
  `resName` varchar(40) NOT NULL,
  `resDesc` varchar(500) DEFAULT NULL,
  `iconPath` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of resource
-- ----------------------------

-- ----------------------------
-- Table structure for serverinfo
-- ----------------------------
DROP TABLE IF EXISTS `serverinfo`;
CREATE TABLE `serverinfo` (
  `serverID` varchar(40) NOT NULL,
  `serverName` varchar(100) NOT NULL,
  `serverDesc` varchar(100) NOT NULL,
  `serverIP1` varchar(20) NOT NULL,
  `serverIP2` varchar(20) NOT NULL,
  `serverUrl` varchar(100) NOT NULL,
  `serverGroup` varchar(20) NOT NULL,
  `serverKey` varchar(40) NOT NULL,
  `partnerId` varchar(40) NOT NULL,
  `serverOpenTime` timestamp NULL DEFAULT NULL COMMENT '????',
  PRIMARY KEY (`serverID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??????';

-- ----------------------------
-- Records of serverinfo
-- ----------------------------

-- ----------------------------
-- Table structure for shop
-- ----------------------------
DROP TABLE IF EXISTS `shop`;
CREATE TABLE `shop` (
  `shopId` int(16) NOT NULL DEFAULT '0',
  `itemId` int(16) DEFAULT '0',
  `tagId` int(2) DEFAULT '0',
  `shopType` int(2) DEFAULT '0',
  `singleNum` int(4) DEFAULT '0',
  `singlePrice` int(8) DEFAULT '0',
  `isPayPointCanBuy` int(2) DEFAULT '0',
  PRIMARY KEY (`shopId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of shop
-- ----------------------------

-- ----------------------------
-- Table structure for skill
-- ----------------------------
DROP TABLE IF EXISTS `skill`;
CREATE TABLE `skill` (
  `skillId` int(16) NOT NULL DEFAULT '0',
  `skillName` varchar(40) NOT NULL,
  `skillDesc` varchar(500) NOT NULL,
  `skillType` varchar(20) NOT NULL,
  `skillLv` int(2) NOT NULL DEFAULT '0',
  `forFight` int(2) NOT NULL DEFAULT '0',
  `sendArmyBinary` varchar(20) NOT NULL,
  `acceptArmyBinary` varchar(20) NOT NULL,
  `cdRoundNum` int(4) NOT NULL DEFAULT '0',
  `isRoundSkill` int(2) NOT NULL DEFAULT '0',
  `minHeroLv` int(2) NOT NULL DEFAULT '0',
  `skillPoint` int(8) NOT NULL DEFAULT '0',
  `impact1Id` varchar(40) NOT NULL,
  `impact2Id` varchar(40) NOT NULL,
  `iconPath` varchar(100) DEFAULT NULL,
  `specAtt` int(2) DEFAULT '0',
  PRIMARY KEY (`skillId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of skill
-- ----------------------------

-- ----------------------------
-- Table structure for state
-- ----------------------------
DROP TABLE IF EXISTS `state`;
CREATE TABLE `state` (
  `stateId` int(16) NOT NULL AUTO_INCREMENT,
  `countryId` int(16) NOT NULL,
  `stateName` varchar(100) NOT NULL,
  `stateNo` int(2) NOT NULL DEFAULT '0',
  `status` int(2) DEFAULT '0',
  `minPosX` int(4) DEFAULT '0',
  `minPosY` int(4) DEFAULT '0',
  `maxPosX` int(4) DEFAULT '0',
  `maxPosY` int(4) DEFAULT '0',
  PRIMARY KEY (`stateId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of state
-- ----------------------------

-- ----------------------------
-- Table structure for strategycondition
-- ----------------------------
DROP TABLE IF EXISTS `strategycondition`;
CREATE TABLE `strategycondition` (
  `conditionId` int(16) NOT NULL DEFAULT '0' COMMENT 'id',
  `strategyId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `conditionName` varchar(100) NOT NULL COMMENT '????',
  `conditionType` int(2) NOT NULL DEFAULT '0' COMMENT '????',
  `description` varchar(500) NOT NULL COMMENT '????',
  `value` int(16) NOT NULL DEFAULT '0' COMMENT '?',
  `errorInfo` varchar(200) NOT NULL COMMENT '??????',
  PRIMARY KEY (`conditionId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of strategycondition
-- ----------------------------

-- ----------------------------
-- Table structure for syspara
-- ----------------------------
DROP TABLE IF EXISTS `syspara`;
CREATE TABLE `syspara` (
  `paraId` varchar(40) NOT NULL,
  `paraGroup` varchar(40) NOT NULL,
  `paraType` varchar(40) NOT NULL,
  `paraValue` varchar(80) NOT NULL,
  `paraName` varchar(40) NOT NULL,
  `paraDesc` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`paraId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of syspara
-- ----------------------------

-- ----------------------------
-- Table structure for tbrealonline
-- ----------------------------
DROP TABLE IF EXISTS `tbrealonline`;
CREATE TABLE `tbrealonline` (
  `dtStatTime` datetime NOT NULL DEFAULT '1970-01-01 00:00:00' COMMENT '????',
  `iWorldId` int(16) NOT NULL DEFAULT '0' COMMENT '????? ? Cluster ID 255',
  `iChannelId` int(16) NOT NULL DEFAULT '0' COMMENT '??ID,ZoneId, ??????65535',
  `iUserNum` int(16) DEFAULT '0' COMMENT '???',
  PRIMARY KEY (`dtStatTime`,`iWorldId`,`iChannelId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tbrealonline
-- ----------------------------

-- ----------------------------
-- Table structure for techeffect
-- ----------------------------
DROP TABLE IF EXISTS `techeffect`;
CREATE TABLE `techeffect` (
  `techEntId` int(16) NOT NULL,
  `level` int(4) NOT NULL DEFAULT '0',
  `effectId` varchar(40) NOT NULL,
  `para1` int(4) NOT NULL DEFAULT '0',
  `para2` int(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`techEntId`,`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of techeffect
-- ----------------------------

-- ----------------------------
-- Table structure for technology
-- ----------------------------
DROP TABLE IF EXISTS `technology`;
CREATE TABLE `technology` (
  `entId` int(16) NOT NULL,
  `techType` varchar(40) DEFAULT NULL,
  `techName` varchar(20) DEFAULT NULL,
  `techDesc` varchar(100) DEFAULT NULL,
  `iconPath` varchar(100) DEFAULT NULL,
  `maxLevel` int(4) DEFAULT '0',
  `orderIndex` int(2) DEFAULT '0',
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of technology
-- ----------------------------

-- ----------------------------
-- Table structure for templatestrategy
-- ----------------------------
DROP TABLE IF EXISTS `templatestrategy`;
CREATE TABLE `templatestrategy` (
  `strategyId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `higherLevelStrategyId` int(16) NOT NULL DEFAULT '0' COMMENT '?????id',
  `name` varchar(100) NOT NULL COMMENT '????',
  `scriptName` varchar(20) NOT NULL COMMENT '????',
  `logo` varchar(100) NOT NULL COMMENT '????',
  `category` int(2) NOT NULL DEFAULT '0' COMMENT '?????(1-??;2-??)',
  `categoryName` varchar(40) NOT NULL COMMENT '???????',
  `type` int(2) NOT NULL DEFAULT '0' COMMENT '?????',
  `typeName` varchar(40) NOT NULL COMMENT '???????',
  `highestProficiency` int(16) NOT NULL DEFAULT '0' COMMENT '???????',
  `lowestProficiency` int(16) NOT NULL DEFAULT '0' COMMENT '????????',
  `level` int(4) NOT NULL DEFAULT '0' COMMENT '????',
  `defaultLevel` int(4) NOT NULL DEFAULT '0' COMMENT '????',
  `topLevel` int(4) NOT NULL DEFAULT '0' COMMENT '????',
  `effectId` varchar(100) NOT NULL COMMENT '??',
  `userLevel` int(4) DEFAULT '0' COMMENT '??????',
  `consumption` int(8) NOT NULL DEFAULT '0' COMMENT '??????',
  `coolingTime` int(16) NOT NULL DEFAULT '0' COMMENT '????(?)',
  `coolingTimeGroupId` int(4) NOT NULL DEFAULT '0' COMMENT '?????id',
  `description` varchar(500) NOT NULL COMMENT '??',
  `limits` varchar(200) NOT NULL COMMENT '????',
  PRIMARY KEY (`strategyId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??';

-- ----------------------------
-- Records of templatestrategy
-- ----------------------------

-- ----------------------------
-- Table structure for tipmessage
-- ----------------------------
DROP TABLE IF EXISTS `tipmessage`;
CREATE TABLE `tipmessage` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `message` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of tipmessage
-- ----------------------------

-- ----------------------------
-- Table structure for titleofgovpotznum
-- ----------------------------
DROP TABLE IF EXISTS `titleofgovpotznum`;
CREATE TABLE `titleofgovpotznum` (
  `id` int(16) NOT NULL DEFAULT '0',
  `titleId` int(16) DEFAULT '0',
  `govPotzId` int(16) DEFAULT '0',
  `num` int(4) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of titleofgovpotznum
-- ----------------------------

-- ----------------------------
-- Table structure for towerarmy
-- ----------------------------
DROP TABLE IF EXISTS `towerarmy`;
CREATE TABLE `towerarmy` (
  `entId` int(16) NOT NULL DEFAULT '0',
  `armyName` varchar(20) DEFAULT NULL,
  `armyDesc` varchar(200) DEFAULT NULL,
  `armyType` varchar(40) DEFAULT NULL,
  `iconPath` varchar(40) DEFAULT NULL,
  `damageNum` int(8) DEFAULT '0',
  `wallSpace` int(8) DEFAULT '0',
  `attackType` varchar(40) DEFAULT NULL,
  `attackRange` int(4) DEFAULT '0',
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of towerarmy
-- ----------------------------

-- ----------------------------
-- Table structure for trackcombat
-- ----------------------------
DROP TABLE IF EXISTS `trackcombat`;
CREATE TABLE `trackcombat` (
  `code` int(16) NOT NULL AUTO_INCREMENT,
  `accUserId` int(16) NOT NULL,
  `appUserId` int(16) NOT NULL,
  `count` int(8) NOT NULL,
  `updateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`code`),
  KEY `idx_accUserId` (`accUserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of trackcombat
-- ----------------------------

-- ----------------------------
-- Table structure for tradeline
-- ----------------------------
DROP TABLE IF EXISTS `tradeline`;
CREATE TABLE `tradeline` (
  `casId` int(16) NOT NULL COMMENT '????Id ??????',
  `endTime` timestamp NULL DEFAULT NULL COMMENT '??????',
  `sumRate` int(16) DEFAULT '0' COMMENT '????  ?????',
  `status` int(2) DEFAULT '0' COMMENT '????\r\n            1,???;2,???;3,???',
  PRIMARY KEY (`casId`),
  KEY `idx_casId` (`casId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????';

-- ----------------------------
-- Records of tradeline
-- ----------------------------

-- ----------------------------
-- Table structure for trans
-- ----------------------------
DROP TABLE IF EXISTS `trans`;
CREATE TABLE `trans` (
  `transId` int(16) NOT NULL AUTO_INCREMENT,
  `tSUsId` int(16) NOT NULL COMMENT '?????userid',
  `tDesUsId` int(16) NOT NULL COMMENT '?????userid',
  `sComment` varchar(800) DEFAULT NULL COMMENT '???????',
  `desComment` varchar(800) DEFAULT NULL COMMENT '???????',
  `tCreatDttm` timestamp NULL DEFAULT NULL COMMENT '??????',
  `tDropDttm` timestamp NULL DEFAULT NULL COMMENT '??????',
  `tLstMfyDttm` timestamp NULL DEFAULT NULL COMMENT '??????',
  `tSuStus` int(2) DEFAULT '0' COMMENT '??????? :0:??? 1:???? 2:??? 3:??? 4:???',
  `tDuStus` char(10) DEFAULT NULL COMMENT '??????? :0:??? 1:???? 2:??? 3:??? 4:???',
  PRIMARY KEY (`transId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of trans
-- ----------------------------

-- ----------------------------
-- Table structure for transitem
-- ----------------------------
DROP TABLE IF EXISTS `transitem`;
CREATE TABLE `transitem` (
  `transItemId` int(16) NOT NULL AUTO_INCREMENT,
  `transId` int(16) NOT NULL,
  `entityId` int(16) NOT NULL COMMENT '????id',
  `count` int(16) DEFAULT '0' COMMENT '????',
  `type` int(2) DEFAULT '0' COMMENT '????:1:??????? 2:???????  (???????????????,?????????)',
  `tStatus` int(2) DEFAULT '0',
  PRIMARY KEY (`transItemId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of transitem
-- ----------------------------

-- ----------------------------
-- Table structure for treasurebox
-- ----------------------------
DROP TABLE IF EXISTS `treasurebox`;
CREATE TABLE `treasurebox` (
  `entId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `dropPackLst` varchar(500) DEFAULT NULL COMMENT '?????',
  `hasKey` int(2) DEFAULT '0' COMMENT '??????(0-???;1-??)',
  `keyEntId` int(8) DEFAULT '0' COMMENT '??(??id)',
  `certainDropItemNum` int(8) DEFAULT '0' COMMENT '?????',
  `probableDropItemNum` int(8) DEFAULT '0' COMMENT '?????',
  `dropType` int(2) DEFAULT '0' COMMENT '????(1-??????????;2-?????????;3-??????;4-?????????,????????;5-?????????,????????)',
  PRIMARY KEY (`entId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????';

-- ----------------------------
-- Records of treasurebox
-- ----------------------------

-- ----------------------------
-- Table structure for treasury
-- ----------------------------
DROP TABLE IF EXISTS `treasury`;
CREATE TABLE `treasury` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) NOT NULL,
  `itemId` int(16) NOT NULL,
  `itemType` int(2) DEFAULT '0',
  `itemCount` int(4) DEFAULT '0',
  `useCount` int(4) DEFAULT '0',
  `band` int(2) DEFAULT '0',
  `equip` int(2) DEFAULT '0',
  `throwAble` int(2) DEFAULT '0',
  `childType` int(4) DEFAULT '0',
  `equipStrongerId` int(16) DEFAULT '0',
  `existEndTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `index_Treasury_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of treasury
-- ----------------------------

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `userId` int(16) NOT NULL AUTO_INCREMENT,
  `accId` bigint(16) NOT NULL,
  `userName` varchar(20) DEFAULT NULL,
  `sex` int(2) DEFAULT '0',
  `icon` varchar(40) DEFAULT NULL,
  `rankId` int(2) NOT NULL DEFAULT '0',
  `waitQueLen` int(2) DEFAULT NULL,
  `buiQueLen` int(2) DEFAULT NULL,
  `cash` int(8) DEFAULT '0',
  `skillPoint` int(8) DEFAULT '0',
  `headIcon` varchar(40) DEFAULT NULL,
  `honor` int(16) DEFAULT '0',
  `titleId` int(16) DEFAULT '0',
  `titleExpireDttm` timestamp NULL DEFAULT NULL,
  `expPoint` int(16) DEFAULT '0',
  `influence` int(16) DEFAULT '0',
  `militaryPoint` int(16) DEFAULT '0',
  `intelPoint` int(16) DEFAULT '0',
  `polityPoint` int(16) DEFAULT '0',
  `availPropertyPoint` int(16) DEFAULT '0',
  `lastLoginDttm` timestamp NULL DEFAULT NULL,
  `payPoint` int(16) DEFAULT '0',
  `achieve` int(16) DEFAULT '0',
  `casNum` int(8) DEFAULT '1',
  `protectStatus` int(2) DEFAULT '0',
  `protectEndDttm` timestamp NULL DEFAULT NULL,
  `tacticPoint` int(16) DEFAULT '0',
  `warStatus` int(2) DEFAULT '0',
  `hexagram` int(16) DEFAULT '0',
  `constituteNum` int(16) DEFAULT '0',
  `laveNum` int(16) DEFAULT '0',
  `currGuide` varchar(200) DEFAULT NULL,
  `honorNum` int(8) DEFAULT '0',
  `hexDate` timestamp NULL DEFAULT NULL,
  `usedActPoint` int(16) DEFAULT '0',
  `lastActDmDttm` timestamp NULL DEFAULT NULL,
  `guildAward` int(2) DEFAULT '0',
  `dailyGiftDttm` timestamp NULL DEFAULT NULL,
  `partnerId` varchar(8) DEFAULT NULL,
  `junGong` int(8) DEFAULT '0',
  `createDate` timestamp NULL DEFAULT NULL,
  `creditDate` timestamp NULL DEFAULT NULL,
  `olTime` int(16) DEFAULT '0',
  `defUser` int(2) DEFAULT '1',
  `prestige` int(16) DEFAULT '0',
  `lasAwardDttm` timestamp NULL DEFAULT NULL,
  `prayTimes` int(16) DEFAULT '0' COMMENT '????????',
  `selfSignature` varchar(200) DEFAULT '‘’' COMMENT '????',
  `pvpNum` int(4) DEFAULT '0' COMMENT '??????',
  `countryId` int(4) DEFAULT '0' COMMENT '??id',
  PRIMARY KEY (`userId`),
  UNIQUE KEY `userName` (`userName`),
  KEY `index_User_accId` (`accId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='??';

-- ----------------------------
-- Records of user
-- ----------------------------

-- ----------------------------
-- Table structure for userachieve
-- ----------------------------
DROP TABLE IF EXISTS `userachieve`;
CREATE TABLE `userachieve` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0' COMMENT '??id',
  `achieveId` int(16) DEFAULT '0' COMMENT '??id',
  `finishStatus` int(2) DEFAULT '0' COMMENT '????(?????)',
  `finishDttm` timestamp NULL DEFAULT NULL COMMENT '??????',
  `subType` varchar(64) DEFAULT '‘combatPve’' COMMENT '??????',
  PRIMARY KEY (`id`),
  KEY `index_UserAchieve_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='?????????';

-- ----------------------------
-- Records of userachieve
-- ----------------------------

-- ----------------------------
-- Table structure for userbabel
-- ----------------------------
DROP TABLE IF EXISTS `userbabel`;
CREATE TABLE `userbabel` (
  `userId` int(16) NOT NULL DEFAULT '0',
  `freeBabelTimes` int(4) DEFAULT '1',
  `itemBabelTimes` int(4) DEFAULT '3',
  `totalScore` int(16) DEFAULT '0',
  `reliveTimes` int(4) DEFAULT '3',
  `winTimes` int(16) DEFAULT '0',
  `stageId` int(8) DEFAULT '0',
  `score` int(16) DEFAULT '0',
  `heroIds` varchar(200) DEFAULT NULL,
  `topStageId` int(16) DEFAULT '0',
  `lastAward` varchar(500) DEFAULT NULL,
  `combating` int(2) DEFAULT '0',
  `autoConfig` varchar(200) DEFAULT NULL,
  `lastWinStatus` int(2) DEFAULT '0',
  `state` int(2) DEFAULT '0',
  `countryName` varchar(20) DEFAULT NULL,
  `topDttm` timestamp NULL DEFAULT NULL,
  `isPass` int(2) DEFAULT '0',
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='????????';

-- ----------------------------
-- Records of userbabel
-- ----------------------------

-- ----------------------------
-- Table structure for userdailymission
-- ----------------------------
DROP TABLE IF EXISTS `userdailymission`;
CREATE TABLE `userdailymission` (
  `uDmId` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `dMId` int(16) DEFAULT '0',
  `casId` int(16) DEFAULT '0',
  `startTime` timestamp NULL DEFAULT NULL,
  `mEndTime` timestamp NULL DEFAULT NULL,
  `status` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`uDmId`),
  KEY `UserDailyMission_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of userdailymission
-- ----------------------------

-- ----------------------------
-- Table structure for usereffect
-- ----------------------------
DROP TABLE IF EXISTS `usereffect`;
CREATE TABLE `usereffect` (
  `playerEffId` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0' COMMENT '??id',
  `effectId` varchar(40) DEFAULT NULL COMMENT '??id',
  `type` varchar(40) DEFAULT NULL COMMENT '????(?????...)',
  `itemEffectId` int(16) DEFAULT '0' COMMENT '????id',
  `absValue` int(8) DEFAULT '0' COMMENT '?????',
  `perValue` int(8) DEFAULT '0' COMMENT '?????',
  `showFlag` int(2) DEFAULT '0' COMMENT '(?????)',
  `expireDttm` timestamp NULL DEFAULT NULL COMMENT '??????',
  PRIMARY KEY (`playerEffId`),
  KEY `index_UserEffect_userId` (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of usereffect
-- ----------------------------

-- ----------------------------
-- Table structure for userevent
-- ----------------------------
DROP TABLE IF EXISTS `userevent`;
CREATE TABLE `userevent` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `eventId` varchar(40) DEFAULT NULL,
  `num` int(2) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `index_UserEvent_userId` (`userId`),
  KEY `index_UserEvent_evnetId` (`eventId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of userevent
-- ----------------------------

-- ----------------------------
-- Table structure for userfarmarea
-- ----------------------------
DROP TABLE IF EXISTS `userfarmarea`;
CREATE TABLE `userfarmarea` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `posX` int(2) DEFAULT '0',
  `posY` int(2) DEFAULT '0',
  `state` int(2) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='????';

-- ----------------------------
-- Records of userfarmarea
-- ----------------------------

-- ----------------------------
-- Table structure for userfarmplant
-- ----------------------------
DROP TABLE IF EXISTS `userfarmplant`;
CREATE TABLE `userfarmplant` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `farmAreaId` int(16) DEFAULT '0',
  `farmTypeId` int(16) DEFAULT '0',
  `userId` int(16) DEFAULT '0',
  `status` int(2) DEFAULT '0',
  `startTime` timestamp NULL DEFAULT NULL,
  `yeild` int(8) DEFAULT '0',
  `standYeild` int(8) DEFAULT '0',
  `stealUserId` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='???????????';

-- ----------------------------
-- Records of userfarmplant
-- ----------------------------

-- ----------------------------
-- Table structure for userfarmtree
-- ----------------------------
DROP TABLE IF EXISTS `userfarmtree`;
CREATE TABLE `userfarmtree` (
  `userId` int(16) NOT NULL AUTO_INCREMENT,
  `waterNum` int(2) DEFAULT '0',
  `waterUser` varchar(200) DEFAULT NULL,
  `maturnTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='??????????';

-- ----------------------------
-- Records of userfarmtree
-- ----------------------------

-- ----------------------------
-- Table structure for userfarmtype
-- ----------------------------
DROP TABLE IF EXISTS `userfarmtype`;
CREATE TABLE `userfarmtype` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `plantName` varchar(40) DEFAULT NULL,
  `resType` int(2) DEFAULT '0',
  `needTime` int(16) DEFAULT '0',
  `outputNum` int(8) DEFAULT '0',
  `itemId` int(16) DEFAULT '0',
  `itemNum` int(2) DEFAULT '0',
  `isNeedItem` int(2) DEFAULT '0',
  `type` int(2) DEFAULT '0',
  `popNum` int(4) DEFAULT '0',
  `seedTime` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='????????';

-- ----------------------------
-- Records of userfarmtype
-- ----------------------------

-- ----------------------------
-- Table structure for userfuncmission
-- ----------------------------
DROP TABLE IF EXISTS `userfuncmission`;
CREATE TABLE `userfuncmission` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `missionId` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of userfuncmission
-- ----------------------------

-- ----------------------------
-- Table structure for usergovpotz
-- ----------------------------
DROP TABLE IF EXISTS `usergovpotz`;
CREATE TABLE `usergovpotz` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `govPotzId` int(16) DEFAULT '0',
  `giveNum` int(2) DEFAULT '0',
  `potzEndTime` timestamp NULL DEFAULT NULL,
  `allNum` int(2) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of usergovpotz
-- ----------------------------

-- ----------------------------
-- Table structure for userguide
-- ----------------------------
DROP TABLE IF EXISTS `userguide`;
CREATE TABLE `userguide` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `currGuideId` int(16) DEFAULT '0',
  `status` int(2) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of userguide
-- ----------------------------

-- ----------------------------
-- Table structure for userguild
-- ----------------------------
DROP TABLE IF EXISTS `userguild`;
CREATE TABLE `userguild` (
  `userId` int(16) NOT NULL DEFAULT '0',
  `guildId` int(16) DEFAULT '0',
  `dutyOfGuild` int(2) DEFAULT '0',
  `pointOfOffer` int(16) DEFAULT '0',
  `goldOfOffer` int(16) DEFAULT '0',
  `fireTimes` int(2) DEFAULT '0',
  `state` int(2) DEFAULT '0',
  `changeLeaderDttm` timestamp NULL DEFAULT NULL,
  `changeLeaderId` int(16) DEFAULT '0',
  `donateRes` int(16) DEFAULT '0',
  `donateItem` int(16) DEFAULT '0',
  `todayDonateRes` int(16) DEFAULT '0',
  `todayDonateItem` int(16) DEFAULT '0',
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='?????';

-- ----------------------------
-- Records of userguild
-- ----------------------------

-- ----------------------------
-- Table structure for userguildtradescope
-- ----------------------------
DROP TABLE IF EXISTS `userguildtradescope`;
CREATE TABLE `userguildtradescope` (
  `id` int(16) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `casId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `destCasIdLst` varchar(210) DEFAULT '' COMMENT '???????id',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of userguildtradescope
-- ----------------------------

-- ----------------------------
-- Table structure for userherobabel
-- ----------------------------
DROP TABLE IF EXISTS `userherobabel`;
CREATE TABLE `userherobabel` (
  `userId` int(16) NOT NULL DEFAULT '0',
  `battleId` int(16) DEFAULT '0',
  `stageId` int(16) DEFAULT '0',
  `freeBabelTimes` int(2) DEFAULT '0',
  `itemBabelTimes` int(2) DEFAULT '0',
  `reliveTimes` int(2) DEFAULT '0',
  `itemNum` int(4) DEFAULT '0',
  `heroId` int(16) DEFAULT '0',
  `topBattleId` int(16) DEFAULT '0',
  `topStageId` int(16) DEFAULT '0',
  `combating` int(2) DEFAULT '0',
  `lastWinStatus` int(2) DEFAULT '0',
  `lastItem` int(4) DEFAULT '0',
  `state` int(2) DEFAULT '0',
  `treatHero` int(2) DEFAULT '0',
  `stopStage` int(4) DEFAULT '0',
  `isPass` int(2) DEFAULT '0',
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='????????';

-- ----------------------------
-- Records of userherobabel
-- ----------------------------

-- ----------------------------
-- Table structure for userlock
-- ----------------------------
DROP TABLE IF EXISTS `userlock`;
CREATE TABLE `userlock` (
  `userId` int(16) NOT NULL AUTO_INCREMENT,
  `version` bigint(20) DEFAULT '0',
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='???';

-- ----------------------------
-- Records of userlock
-- ----------------------------

-- ----------------------------
-- Table structure for userluck
-- ----------------------------
DROP TABLE IF EXISTS `userluck`;
CREATE TABLE `userluck` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `luckType` int(16) DEFAULT '0',
  `luckValue` int(16) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='??????';

-- ----------------------------
-- Records of userluck
-- ----------------------------

-- ----------------------------
-- Table structure for usermission
-- ----------------------------
DROP TABLE IF EXISTS `usermission`;
CREATE TABLE `usermission` (
  `userMissionId` int(16) NOT NULL AUTO_INCREMENT,
  `missionId` int(16) DEFAULT '0',
  `userId` int(16) DEFAULT '0',
  `octType` varchar(40) DEFAULT NULL,
  `completeLimitTime` timestamp NULL DEFAULT NULL,
  `status` int(2) DEFAULT '0',
  `count` int(2) DEFAULT '0',
  `num1` int(8) DEFAULT '0',
  `num2` int(8) DEFAULT '0',
  `num3` int(8) DEFAULT '0',
  `num4` int(8) DEFAULT '0',
  `num5` int(8) DEFAULT '0',
  `mapPosition` varchar(40) DEFAULT NULL,
  `missionType` varchar(40) DEFAULT NULL,
  `detect` int(2) DEFAULT '0',
  PRIMARY KEY (`userMissionId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of usermission
-- ----------------------------

-- ----------------------------
-- Table structure for usermissionlimit
-- ----------------------------
DROP TABLE IF EXISTS `usermissionlimit`;
CREATE TABLE `usermissionlimit` (
  `userId` int(16) NOT NULL DEFAULT '0',
  `heroMissionCount` int(4) DEFAULT '0',
  `heroMissionFreshTime` timestamp NULL DEFAULT NULL,
  `missionFreshTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of usermissionlimit
-- ----------------------------

-- ----------------------------
-- Table structure for usernewplayerlead
-- ----------------------------
DROP TABLE IF EXISTS `usernewplayerlead`;
CREATE TABLE `usernewplayerlead` (
  `userLeadId` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `leadId` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`userLeadId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of usernewplayerlead
-- ----------------------------

-- ----------------------------
-- Table structure for usernpcanimus
-- ----------------------------
DROP TABLE IF EXISTS `usernpcanimus`;
CREATE TABLE `usernpcanimus` (
  `userId` int(16) NOT NULL DEFAULT '0',
  `baomin` int(8) DEFAULT '0',
  `winNum1` int(8) DEFAULT '0',
  `isAttack1` int(2) DEFAULT '0',
  `qiangdao` int(8) DEFAULT '0',
  `winNum2` int(8) DEFAULT '0',
  `isAttack2` int(2) DEFAULT '0',
  `tufei` int(8) DEFAULT '0',
  `winNum3` int(8) DEFAULT '0',
  `isAttack3` int(2) DEFAULT '0',
  `panjun` int(8) DEFAULT '0',
  `winNum4` int(8) DEFAULT '0',
  `isAttack4` int(2) DEFAULT '0',
  `jindi` int(8) DEFAULT '0',
  `winNum5` int(8) DEFAULT '0',
  `isAttack5` int(2) DEFAULT '0',
  `lastAddTime` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of usernpcanimus
-- ----------------------------

-- ----------------------------
-- Table structure for userranking
-- ----------------------------
DROP TABLE IF EXISTS `userranking`;
CREATE TABLE `userranking` (
  `userId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `casLv` int(4) DEFAULT '1' COMMENT '????',
  `rangeValue` int(16) DEFAULT '50' COMMENT '?????',
  `userLv` int(4) DEFAULT '1' COMMENT '????',
  `leagueLv` int(4) DEFAULT '0' COMMENT '??????',
  `seasonIntegral` int(16) DEFAULT '0' COMMENT '????',
  `seansonWinNum` int(4) DEFAULT '0' COMMENT '?????',
  `seasonLoseNum` int(4) DEFAULT '0' COMMENT '?????',
  PRIMARY KEY (`userId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='????';

-- ----------------------------
-- Records of userranking
-- ----------------------------

-- ----------------------------
-- Table structure for userrankingtmp
-- ----------------------------
DROP TABLE IF EXISTS `userrankingtmp`;
CREATE TABLE `userrankingtmp` (
  `rankId` int(16) NOT NULL DEFAULT '0' COMMENT 'id',
  `userId` int(16) DEFAULT '0' COMMENT '??id',
  `casLv` int(4) DEFAULT '1' COMMENT '????',
  `rangeValue` int(16) DEFAULT '0' COMMENT '?????',
  `userLv` int(4) DEFAULT '1' COMMENT '????',
  PRIMARY KEY (`rankId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='????(???)';

-- ----------------------------
-- Records of userrankingtmp
-- ----------------------------

-- ----------------------------
-- Table structure for userstrategy
-- ----------------------------
DROP TABLE IF EXISTS `userstrategy`;
CREATE TABLE `userstrategy` (
  `id` int(16) NOT NULL AUTO_INCREMENT COMMENT 'id',
  `userId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `strategyId` int(16) NOT NULL DEFAULT '0' COMMENT '??id',
  `level` int(4) NOT NULL DEFAULT '0' COMMENT '??????',
  `proficiency` int(16) NOT NULL DEFAULT '0' COMMENT '???????',
  `lastCoolDeadline` timestamp NULL DEFAULT NULL COMMENT '?????????',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='????';

-- ----------------------------
-- Records of userstrategy
-- ----------------------------

-- ----------------------------
-- Table structure for usertask
-- ----------------------------
DROP TABLE IF EXISTS `usertask`;
CREATE TABLE `usertask` (
  `id` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) DEFAULT '0',
  `taskId` int(16) DEFAULT '0',
  `taskType` int(2) DEFAULT '0',
  `finishDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk COMMENT='??????????';

-- ----------------------------
-- Records of usertask
-- ----------------------------

-- ----------------------------
-- Table structure for usertech
-- ----------------------------
DROP TABLE IF EXISTS `usertech`;
CREATE TABLE `usertech` (
  `userTechId` int(16) NOT NULL AUTO_INCREMENT,
  `userId` int(16) NOT NULL,
  `techEntId` int(16) NOT NULL,
  `level` int(4) NOT NULL DEFAULT '0',
  `opDttm` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`userTechId`)
) ENGINE=InnoDB DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of usertech
-- ----------------------------
