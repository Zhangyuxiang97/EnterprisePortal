// -*- coding: utf-8 -*-
const fs = require('fs');
const path = require('path');

// 读取数据
const dataPath = path.join(__dirname, 'data', 'budget-tasks.json');
const data = JSON.parse(fs.readFileSync(dataPath, 'utf-8'));

const results = [];

for (const item of data) {
    const itemId = item.id;
    const title = item.title.substring(0, 50);
    const content = item.content || '';

    let canInfer = false;
    let budget = null;
    let unit = null;
    let reason = '正文中未找到明确的金额信息';

    if (!content || content.trim().length < 10) {
        reason = '正文内容过短或为空，无法提取金额信息';
    } else {
        let found = false;

        // 先检查百分比形式的控制价（不能推算）
        const percentPatterns = [
            /预算金额[：:]\s*餐费的\d+%/,
            /控制价[：:]\s*施工中标价的\d+%/,
            /批发价的\d+%/,
            /市场上相应食品原材料批发价的\d+%/,
        ];

        for (const pattern of percentPatterns) {
            if (pattern.test(content)) {
                reason = '控制价为百分比形式，无法推算具体金额';
                found = true;
                break;
            }
        }

        if (!found) {
            // 检查是否是单价形式（元/平方米、元/台/月等）
            const unitPricePatterns = [
                /(\d+(?:\.\d+)?)\s*元\/平方米/,
                /(\d+(?:\.\d+)?)\s*元\/台\/月/,
            ];

            let hasUnitPrice = false;
            let unitPriceValue = null;

            for (const pattern of unitPricePatterns) {
                const match = content.match(pattern);
                if (match) {
                    hasUnitPrice = true;
                    unitPriceValue = parseFloat(match[1].replace(/,/g, ''));
                    break;
                }
            }

            if (hasUnitPrice) {
                // 尝试查找面积
                const areaMatch = content.match(/约(\d+(?:,\d{3})*)\s*平方米/);
                if (areaMatch) {
                    const areaValue = parseFloat(areaMatch[1].replace(/,/g, ''));
                    const total = unitPriceValue * areaValue;
                    canInfer = true;
                    budget = Math.round(total * 100) / 100; // 保留2位小数
                    unit = '元';
                    reason = `单价${unitPriceValue}元/平方米 × 面积约${areaValue}平方米 ≈ ${Math.round(total * 100) / 100}元`;
                } else {
                    reason = '为单价形式，无总面积信息，无法推算总金额';
                }
            } else {
                // 查找万元
                const wanYuanPatterns = [
                    /预算金额[：:]\s*约?(\d+(?:,\d{3})*(?:\.\d+)?)\s*万元/,
                    /控制价[：:]\s*(\d+(?:,\d{3})*(?:\.\d+)?)\s*万元/,
                    /概算约(\d+(?:,\d{3})*(?:\.\d+)?)\s*万元/,
                    /总概算约(\d+(?:,\d{3})*(?:\.\d+)?)\s*万元/,
                    /(\d+(?:,\d{3})*(?:\.\d+)?)\s*万元/,
                ];

                for (const pattern of wanYuanPatterns) {
                    const match = content.match(pattern);
                    if (match) {
                        const amountStr = match[1].replace(/,/g, '');
                        const amount = parseFloat(amountStr);
                        canInfer = true;
                        budget = amount;
                        unit = '万元';
                        reason = `文中明确提到金额${amount}万元`;
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    // 查找元
                    const yuanPatterns = [
                        /预算金额[：:]\s*约?￥?\s*(\d+(?:,\d{3})*(?:\.\d+)?)\s*元/,
                        /控制价[：:]\s*(\d+(?:,\d{3})*(?:\.\d+)?)\s*元/,
                        /采购控制价[：:]\s*(\d+(?:,\d{3})*(?:\.\d+)?)\s*元/,
                        /最低限价[：:]\s*(\d+(?:,\d{3})*(?:\.\d+)?)\s*元/,
                    ];

                    for (const pattern of yuanPatterns) {
                        const match = content.match(pattern);
                        if (match) {
                            const amountStr = match[1].replace(/,/g, '');
                            const amount = parseFloat(amountStr);
                            if (amount > 0) {
                                canInfer = true;
                                budget = amount; // 保留原始金额
                                unit = '元';
                                reason = `文中明确提到金额${amount}元`;
                            } else {
                                reason = '控制价为0元，属于特殊项目';
                            }
                            found = true;
                            break;
                        }
                    }
                }

                if (!found) {
                    // 检查是否有"XX万元"格式但没有关键词（排除日期误匹配）
                    const specialPattern = /(\d+(?:\.\d+)?)\s*万元/;
                    let match;
                    let lastIndex = 0;

                    while ((match = specialPattern.exec(content.substring(lastIndex))) !== null) {
                        const actualIndex = lastIndex + match.index;
                        const contextStart = Math.max(0, actualIndex - 20);
                        const contextEnd = Math.min(content.length, actualIndex + match[0].length + 20);
                        const context = content.substring(contextStart, contextEnd);

                        // 排除日期中的"年"误匹配
                        if (context.includes('年') && !context.includes('月') && !context.includes('日')) {
                            lastIndex = actualIndex + match[0].length;
                            continue;
                        }

                        const amountStr = match[1];
                        const amount = parseFloat(amountStr);
                        if (amount > 0) {
                            canInfer = true;
                            budget = amount;
                            unit = '万元';
                            reason = `文中提到金额${amount}万元`;
                            found = true;
                        }
                        break;
                    }
                }
            }
        }
    }

    results.push({
        id: itemId,
        title: title,
        canInfer: canInfer,
        budget: budget,
        unit: unit,
        reason: reason
    });
}

// 保存结果
const outputPath = path.join(__dirname, 'data', 'budget-analysis.json');
fs.writeFileSync(outputPath, JSON.stringify(results, null, 2), 'utf-8');

// 统计
const canInferCount = results.filter(r => r.canInfer).length;
const cannotInferCount = results.length - canInferCount;
console.log('分析完成！');
console.log(`总计: ${results.length} 条`);
console.log(`可推算金额: ${canInferCount} 条`);
console.log(`无法推算: ${cannotInferCount} 条`);
console.log('\n可推算金额的项目:');
for (const r of results) {
    if (r.canInfer) {
        console.log(`  ID ${r.id}: ${r.budget} ${r.unit} - ${r.reason}`);
    }
}
