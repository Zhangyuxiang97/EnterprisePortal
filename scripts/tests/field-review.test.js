const test=require('node:test');
const assert=require('node:assert/strict');
const {loadRegions}=require('../generate-initial-announcement-sql');
const {review,reviewAll,reviewInfo,extractDocument,sumAmounts}=require('../review-announcement-fields');
const regions=loadRegions();
const check=(title,text,tables=[])=>review({id:0,title,text,tables,category:26},regions);

test('服务采购与政府采购工程依正文方式分类',()=>{
  const service=check('物业服务采购项目成交公告','采购人：某单位。竞争性磋商，预算金额：90万元，成交金额：86.9万元。');
  assert.equal(service.fields.business_type.value,'GOV_PROCUREMENT');
  assert.equal(service.fields.procurement_type.value,'service');
  assert.equal(service.fields.budget_amount.value,90);
  assert.equal(service.fields.award_amount.value,86.9);
  assert.equal(check('设备采购公告','采购项目预算金额：277007.73元').fields.budget_amount.value,27.7);
  const project=check('卫生间改造项目成交公告','竞争性磋商；项目投资额：151120.35元；成交金额：148900元。');
  assert.equal(project.fields.business_type.value,'GOV_PROCUREMENT');
  assert.equal(project.fields.procurement_type.value,'project');
  assert.equal(project.fields.budget_amount.value,null);
  assert.equal(project.fields.award_amount.value,14.89);
});

test('控制价、单价、代理费与候选报价不填入预算或中标金额',()=>{
  const row=check('工程监理中标候选人公示','商工程〔2026〕；最高投标限价：90万元；第一中标候选人中标金额：86.9万元；代理费：1000元。');
  assert.equal(row.fields.business_type.value,'CONSTRUCTION');
  assert.equal(row.fields.procurement_type.value,null);
  assert.equal(row.fields.budget_amount.value,null);
  assert.equal(row.fields.award_amount.value,null);
  assert.equal(check('围挡搭建成交公告','预算金额：单价：130元/平方米').fields.budget_amount.value,null);
});

test('表格金额保留单位，多个标段及数量级冲突留空',()=>{
  const tables=extractDocument('<table><tr><td>供应商</td><td>中标金额（元）</td></tr><tr><td>公司</td><td>3280000</td></tr></table>').tables;
  assert.equal(check('设备中标公告','',tables).fields.award_amount.value,328);
  assert.equal(check('设备中标公告','中标金额（元）\n2025-55\n公司',tables).fields.award_amount.value,328);
  const many=check('工程中标结果公告','中标单位：甲\n投标报价：229993318.58元\n第二标段\n中标单位：乙\n投标报价：22998984.14元');
  assert.equal(many.fields.award_amount.value,null);
  const conflict=check('生态修复工程中标结果公告','招标控制价：16980639.52元',[
    [['中标金额（万元）'],['16642323.72']]
  ]);
  assert.equal(conflict.fields.award_amount.value,null);
  assert.ok(conflict.needsReview.includes('中标金额原文单位冲突'));
});

test('地区优先项目地点，忽略道路同名和代理供应商地址',()=>{
  const nanyang=check('南阳市中心城区排水工程招标公告','建设地点：南阳市中心城区北京大道；采购代理机构地址：郑州市金水区。');
  assert.equal(nanyang.fields.city.value,'南阳市');
  assert.equal(nanyang.fields.province.value,'河南省');
  const gongyi=check('巩义市监理招标公告','');
  assert.equal(gongyi.fields.city.value,'郑州市');
  assert.equal(check('太康县农业项目采购公告','').fields.city.value,'周口市');
  assert.equal(check('清丰县农业项目采购公告','').fields.city.value,'濮阳市');
  const buyer=check('河南省设备采购公告','采购人信息\n名称：某研究所\n地址：郑州市二七区嵩山南路169号\n采购代理机构信息\n地址：北京市');
  assert.equal(buyer.fields.city.value,'郑州市');
  assert.equal(check('河南省内黄监狱采购公告','采 购 人：内黄监狱\n地址：河南省安阳市内黄县\n代理机构：某公司\n地址：濮阳市').fields.city.value,'安阳市');
  assert.equal(check('河南省医院采购公告','采购人信息\n名称：医院\n地址：开封市\n财政部门信息\n地址：郑州市').fields.city.value,'开封市');
  assert.equal(check('财政衔接项目中标候选人公示','招标人或招标代理机构\n（异议受理） 单位名称：荥阳市农业农村局\n单位地址：荥阳市富民路25号').fields.city.value,'郑州市');
  const unknown=check('设备采购公告','供应商地址：北京市；采购代理机构地址：郑州市。');
  assert.equal(unknown.fields.city.value,'');
});

test('同名项目可补缺失分类，但不复制金额；分包保留独立性',()=>{
  const rows=reviewAll([
    {id:1,title:'某院信息中心项目招标公告',text:'采购内容：软件开发服务；预算金额：90万元',tables:[],category:1},
    {id:2,title:'某院信息中心项目更正公告',text:'仅变更开标时间。',tables:[],category:27},
    {id:3,title:'某院信息中心项目（包2）终止公告',text:'仅终止采购。',tables:[],category:26}
  ],regions);
  assert.equal(rows[1].fields.procurement_type.value,'service');
  assert.equal(rows[1].fields.budget_amount.value,null);
  assert.equal(rows[2].fields.procurement_type.value,null);
});

test('资讯分类使用当前选项并区分知识解读与政策正文',()=>{
  assert.equal(reviewInfo({id:318,news_title:'PPP法律冲突',news_content:'导读及法律问题点评'}).category,'知识资讯');
  assert.equal(reviewInfo({id:322,news_title:'河南清单招标办法',news_content:'省住房建设主管部门通知'}).category,'地方政策');
  assert.throws(()=>reviewInfo({id:999999}),/尚未逐条确认/);
});

test('预算明确变更后采用新值，结果更正留证而不重复统计',()=>{
  const row=check('规划设计变更公告','原采购信息内容：预算金额：638188.1元\n变更为：预算金额：516788.1元');
  assert.equal(row.fields.budget_amount.value,51.68);
  assert.ok(!row.needsReview.includes('budget 存在多个金额'));
  const unknown=check('预算更正公告','A包预算金额：10万元；B包预算金额：20万元');
  assert.equal(unknown.fields.budget_amount.value,null);
  const correction=check('施工中标结果变更公告','原内容：中标人：甲\n投标报价：2318216.66元\n现变更为：\n中标人：乙\n投标报价：2274129.06元');
  assert.equal(correction.fields.award_amount.value,null);
  assert.equal(correction.notes.find(n=>n.type==='resultCorrection').currentAwardAmount,227.41);
});

test('结果公告取确认成交价，截断其他供应商报价并识别空格中标人',()=>{
  const row=check('古建修缮结果公告','成交供应商：甲\n成交价：224218.61元\n五、主要成交标的\n工程类\n七、供应商报价情况\n响应单位：乙；一次报价：258170.19元');
  assert.equal(row.fields.award_amount.value,22.42);
  assert.equal(check('营区修复成交结果公告','中标（成交）金额：265000元').fields.award_amount.value,26.5);
  assert.equal(row.moneyCandidates.filter(c=>c.kind==='award').length,1);
  assert.equal(check('农田中标公告','中 标 人：甲\n投标报价：2187746.72元').fields.award_amount.value,218.77);
  const missing=check('农田中标公告','中 标 人：甲\n投标报价：2933422.62\n综合得分：89分');
  assert.equal(missing.fields.award_amount.value,null);
  assert.ok(missing.needsReview.includes('最终中标报价缺少金额单位'));
});

test('独立费率单位优先于金额元表头，候选多报价不列未解决金额冲突',()=>{
  const row=check('信息化中标公告','',[[['包号','中标金额（元）','单位'],['1','90545180','元'],['2','0.79','%']]]);
  assert.deepEqual(row.moneyCandidates.filter(c=>c.kind==='award').map(c=>c.sourceNumber),['90545180']);
  const unitPrice=check('校服成交公告','中标金额\n单位\n包1\n供应商名称\n公司\n地址\n710\n元/人\n序号\n名称\n数量\n单价\n1\n校服\n2件\n50元',[
    [['包号','供应商名称','中标金额','单位'],['包1','公司','710','元/人']]
  ]);
  assert.equal(unitPrice.fields.award_amount.value,null);
  const candidate=check('服务中标候选人公示','中标金额：1万元；中标金额：2万元');
  assert.equal(candidate.fields.award_amount.value,null);
  assert.ok(!candidate.needsReview.includes('award 存在多个金额'));
  assert.ok(candidate.notes.some(n=>n.type==='candidateNotice'));
});

test('完整分包范围汇总原数值后舍入，同价不同包不会丢失；范围变化拒绝生成',()=>{
  assert.equal(sumAmounts([{sourceNumber:'40',sourceUnit:'元'},{sourceNumber:'40',sourceUnit:'元'}]),0.01);
  assert.equal(sumAmounts([{sourceNumber:'229993318.58',sourceUnit:'元'},{sourceNumber:'22998984.14',sourceUnit:'元'}]),25299.23);
  const record={id:2705,title:'食品安全抽检项目成交公告',category:26,text:'共划分3个标包',tables:[
    [['包号','供应商名称','中标金额（元）'],['1包','甲','100000'],['2包','乙','100000'],['3包','丙','200000']]
  ]};
  const row=reviewAll([record],regions)[0];
  assert.equal(row.fields.award_amount.value,40);
  assert.equal(row.fields.award_amount.parts.length,3);
  assert.throws(()=>reviewAll([{...record,tables:[record.tables[0].slice(0,3)]}],regions),/已复核范围不符/);
});

test('新增地区标签只读真实项目和采购人，多区县正常留空区县',()=>{
  assert.equal(check('物业遴选招标公告','遴选单位：某公司\n地址：河南省郑州市\n代理机构：海隆公司\n地址：北京市').fields.city.value,'郑州市');
  assert.equal(check('物业采购公告','招标单位：某公司\n地址：二七区嵩山南路151号\n代理机构：某公司\n地址：北京市').fields.city.value,'郑州市');
  assert.equal(check('旧址修缮谈判公告','服务实施地：河南省焦作市').fields.city.value,'焦作市');
  assert.equal(check('中原区围挡项目招标公告','').fields.district.value,'中原区');
  const multi=check('焦作市保护项目公告','项目地点：河南省焦作市修武县、博爱县及沁阳市');
  assert.equal(multi.fields.district.value,'');
  assert.ok(!multi.needsReview.includes('涉及多个地区'));
  assert.ok(multi.notes.some(n=>n.type==='multipleDistricts'));
});

test('同标段标的复核追溯原公告且拒绝变动来源',()=>{
  const target={id:1218,title:'南大桥乡人居环境整治项目（第二标段）终止公告',category:27,text:'终止第二标段',tables:[]};
  const source={id:1211,title:'南大桥乡人居环境整治施工及监理采购公告',category:1,text:'第二标段：本项目及张老埠桥头村乡村会客厅建设项目的监理工作',tables:[]};
  const row=reviewAll([target,source],regions)[0];
  assert.equal(row.fields.procurement_type.value,'service');
  assert.equal(row.fields.procurement_type.sourceId,1211);
  assert.throws(()=>reviewAll([target,{...source,text:'第一标段施工'}],regions),/采购标的复核依据已变化/);
});

test('授权推断只补指定历史公告子类，保留金额并记录推断依据',()=>{
  const records=[
    {id:2165,title:'临颍县城关街道七个农业村人居环境集中治理项目竞争性磋商公告',category:1,
      text:'采购内容：临颍县城关街道七个农业村人居环境集中治理（详见磋商文件）\n合同履行期限：45日历天\n预算金额：220000.00元',tables:[]},
    {id:799,title:'武陟县电子商务发展促进中心电商进农村综合示范采购项目招标文件公示',category:1,
      text:'项目编号：武政招标采购【2021】011号\n标前公示内容：见附件。',tables:[]},
    ...[509,510].map(id=>({id,title:'焦作市解放区融媒体文化科技产业园路演大厅采购项目竞争性磋商公告',category:1,
      text:'项目编号：HLZB-2019-022\n项目基本情况：详见竞争性磋商文件\n项目预算金额：1030000元',tables:[]}))
  ];
  const rows=reviewAll(records,regions);
  assert.deepEqual(rows.map(row=>row.fields.procurement_type.value),['service','service','goods','goods']);
  assert.deepEqual(rows.map(row=>row.fields.budget_amount.value),[22,null,103,103]);
  for(const row of rows) {
    assert.equal(row.fields.procurement_type.inferred,true);
    assert.ok(row.fields.procurement_type.rationale);
    assert.equal(row.fields.award_amount.value,null);
    assert.ok(!row.needsReview.includes('采购标的无法明确识别'));
  }
  assert.throws(()=>reviewAll([{...records[0],text:'变更后的其他采购内容'}],regions),/采购分类推断依据已变化/);
  assert.throws(()=>reviewAll([{...records[0],title:'其他项目竞争性磋商公告'}],regions),/采购分类推断依据已变化/);
  const unrelated=reviewAll([{...records[0],id:9999}],regions)[0];
  assert.equal(unrelated.fields.procurement_type.value,null);
  assert.ok(!unrelated.fields.procurement_type.inferred);
});

test('整项目终止按主要预算包归档，保留混合说明且不复制分包金额',()=>{
  const records=[
    {id:2956,title:'安阳县中医院紧密型医共体资源共享中心项目终止公告',category:26,
      text:'项目编号：AYXYLCG-2026-001\n本项目因故终止采购',tables:[]},
    {id:2912,title:'安阳县中医院紧密型医共体资源共享中心项目（包1）招标公告',category:1,
      text:'采购内容：智慧医院信息化建设一批\n包预算：58000000.00元',tables:[]},
    {id:2913,title:'安阳县中医院紧密型医共体资源共享中心项目（包2）招标公告',category:1,
      text:'采购内容：医疗设备一批\n包预算：79600000.00元',tables:[]}
  ];
  const row=reviewAll(records,regions)[0];
  assert.equal(row.fields.procurement_type.value,'goods');
  assert.equal(row.fields.procurement_type.inferred,true);
  assert.ok(row.notes.some(note=>note.type==='mixedProcurement'));
  assert.equal(row.fields.budget_amount.value,null);
  assert.equal(row.fields.award_amount.value,null);
  assert.ok(!row.needsReview.includes('项目多个包采购类型不同，单一子类字段无法表示'));
  assert.throws(()=>reviewAll(records.slice(0,2),regions),/采购分类推断依据已变化/);
  assert.throws(()=>reviewAll(records.map(record=>record.id===2913?{...record,text:record.text.replace('79600000.00','10000000.00')}:record),regions),/采购分类推断依据已变化/);
});
