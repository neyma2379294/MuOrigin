-- kevinh - the following lines are part of our standard init
-- require("compat-5.1")
-- 补偿
function talk(luaMgr, client, params)
  kaishitime = luaMgr:GetStartBuChangTime(client);
  jieshutime = luaMgr:GetEndBuChangTime(client);
  buchangexp = luaMgr:GetBuChangExp(client);
  buchangbindyuanbao = luaMgr:GetBuChangBindYuanBao(client);
  goodsnames = luaMgr:GetBuChangGoodsNames(client);

xmlHead = '<?xml version="1.0" encoding="utf-8"?>\n<Items>\n';
xmlFoot = '</Items>';

xmlBody = '<Item Title="补偿公告" Text="'
..'维护补偿玩家\\n\\n'
..'《全民奇迹》研发团队在此感谢广大玩家一如既往的支持，祝大家游戏愉快！详细补偿的奖励如下:\\n\\n'
..'{ff0000}每个玩家在补偿活动期间，领取一次补偿！{-}\\n'
..'{ffff00}开始时间：{-}{ffffff}'..kaishitime..'{-}\\n'
..'{ffff00}结束时间：{-}{ffffff}'..jieshutime..'{-}\\n'
..'{ffff00}补偿经验：{-}{00ff00}'..buchangexp..'{-}\\n'
..'{ffff00}补偿魔晶：{-}{ffffff}'..buchangbindyuanbao..'{-}\\n'
..'{ffff00}补偿物品：{-}{ffffff}'..goodsnames..'{-}\\n'
..'" Event="event:givebuchang|获取补偿'
..'"/>'

return string.format("%s%s%s", xmlHead, xmlBody, xmlFoot);
		 
end

function givebuchang(luaMgr, client, params)
     return luaMgr:GiveBuChang(client);
end





