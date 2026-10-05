/**
 * ECharts 图表配置工具函数
 * 提供各种常用图表的配置选项
 */

/**
 * 获取交易类型饼图配置
 * @param {Array} data - 项目类型数据 [{type: '政府采购-货物', count: 680, percentage: 54.05}]
 * @param {Object} options - 自定义配置
 */
export const getPieChartOption = (data, options = {}) => {
  // 为4个类型定义专属颜色
  const colorMap = {
    '建设工程': '#799391',      // 绿色
    '政府采购-货物': '#245b85',  // 蓝色
    '政府采购-服务': '#658eae',  // 橙色
    '政府采购-工程': '#93aec3'   // 紫色
  }
  
  // 根据类型名称分配颜色
  const colors = data.map(item => colorMap[item.type] || '#6B7280')
  
  const defaultOptions = {
    title: '公告业务分类',
    showLegend: true,
    radius: ['38%', '64%'],
    center: ['50%', '46%']
  }
  
  const config = { ...defaultOptions, ...options }
  
  return {
    title: config.title ? {
      text: config.title,
      left: 'center',
      top: 10,
      textStyle: {
        fontSize: 16,
        fontWeight: 'bold'
      }
    } : undefined,
    tooltip: {
      trigger: 'item',
      formatter: '{b}：{c} 条（{d}%）'
    },
    legend: config.showLegend ? {
      orient: 'horizontal',
      left: 'center',
      bottom: 12,
      textStyle: {
        fontSize: 12
      }
    } : undefined,
    series: [
      {
        name: '公告分类',
        type: 'pie',
        radius: config.radius,
        center: config.center,
        avoidLabelOverlap: true,
        itemStyle: {
          borderRadius: 4,
          borderColor: '#fff',
          borderWidth: 2
        },
        label: {
          show: true,
          formatter: '{d}%',
          position: 'inside',
          color: '#fff',
          fontSize: 12
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 14,
            fontWeight: 'bold'
          }
        },
        data: data.map(item => ({
          name: item.type,
          value: item.count,
          label: { show: item.percentage >= 4 }
        }))
      }
    ],
    color: colors
  }
}

/**
 * 获取地区排行柱状图配置
 * @param {Array} data - 地区排行数据 [{region: '郑州市', projectCount: 356, amount: 0}]
 * @param {Object} options - 自定义配置
 */
export const getBarChartOption = (data, options = {}) => {
  const defaultOptions = {
    title: '地区公告排行',
    xAxisLabel: 'region',
    yAxisLabel: 'projectCount',
    color: '#245b85',
    showLabel: true
  }
  
  const config = { ...defaultOptions, ...options }
  const regions = data.map(item => item[config.xAxisLabel])
  const counts = data.map(item => item[config.yAxisLabel])

  return {
    title: config.title ? {
      text: config.title,
      left: 'center',
      top: 10,
      textStyle: {
        fontSize: 16,
        fontWeight: 'bold'
      }
    } : undefined,
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      },
      formatter: '{b}：{c} 条公告'
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      top: 60,
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: regions,
      axisLabel: {
        interval: 0,
        rotate: 30,
        fontSize: 12
      }
    },
    yAxis: {
      type: 'value',
      name: '公告数量',
      axisLabel: {
        formatter: '{value}条'
      }
    },
    series: [
      {
        name: '公告数量',
        type: 'bar',
        data: counts,
        itemStyle: {
          color: config.color,
          borderRadius: [5, 5, 0, 0]
        },
        barWidth: '60%',
        label: config.showLabel ? {
          show: true,
          position: 'top',
          formatter: '{c}条'
        } : undefined
      }
    ]
  }
}

/**
 * 获取折线图配置
 * @param {Object} data - 数据 {dates: [], series: [{name: '', data: []}]}
 * @param {Object} options - 自定义配置
 */
export const getLineChartOption = (data, options = {}) => {
  const defaultOptions = {
    title: '',
    smooth: true,
    showArea: false,
    colors: ['#409eff', '#67c23a', '#e6a23c', '#f56c6c', '#909399']
  }
  
  const config = { ...defaultOptions, ...options }
  
  return {
    title: config.title ? {
      text: config.title,
      left: 'center',
      top: 10,
      textStyle: {
        fontSize: 16,
        fontWeight: 'bold'
      }
    } : undefined,
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'cross'
      }
    },
    legend: {
      data: data.series.map(s => s.name),
      top: config.title ? 40 : 10
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      top: config.title ? 80 : 50,
      containLabel: true
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: data.dates || []
    },
    yAxis: {
      type: 'value'
    },
    series: data.series.map((item, index) => ({
      name: item.name,
      type: 'line',
      data: item.data,
      smooth: config.smooth,
      itemStyle: {
        color: config.colors[index % config.colors.length]
      },
      areaStyle: config.showArea ? {} : undefined
    })),
    color: config.colors
  }
}

/**
 * 获取环形图配置
 * @param {Array} data - 数据 [{name: '', value: 0}]
 * @param {Object} options - 自定义配置
 */
export const getDoughnutChartOption = (data, options = {}) => {
  const defaultOptions = {
    title: '',
    radius: ['50%', '70%'],
    center: ['50%', '50%'],
    showLabel: true
  }
  
  const config = { ...defaultOptions, ...options }
  
  return {
    title: config.title ? {
      text: config.title,
      left: 'center',
      top: 10,
      textStyle: {
        fontSize: 16,
        fontWeight: 'bold'
      }
    } : undefined,
    tooltip: {
      trigger: 'item',
      formatter: '{b}: {c} ({d}%)'
    },
    legend: {
      orient: 'vertical',
      right: 10,
      top: 'center'
    },
    series: [
      {
        type: 'pie',
        radius: config.radius,
        center: config.center,
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 2
        },
        label: {
          show: config.showLabel,
          formatter: '{b}\n{d}%',
          fontSize: 12
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 14,
            fontWeight: 'bold'
          }
        },
        labelLine: {
          show: config.showLabel
        },
        data: data
      }
    ]
  }
}

/**
 * 获取横向柱状图配置
 * @param {Array} data - 数据 [{name: '', value: 0}]
 * @param {Object} options - 自定义配置
 */
export const getHorizontalBarChartOption = (data, options = {}) => {
  const defaultOptions = {
    title: '',
    color: '#5470c6',
    showLabel: true
  }
  
  const config = { ...defaultOptions, ...options }
  const names = data.map(item => item.name)
  const values = data.map(item => item.value)
  
  return {
    title: config.title ? {
      text: config.title,
      left: 'center',
      top: 10,
      textStyle: {
        fontSize: 16,
        fontWeight: 'bold'
      }
    } : undefined,
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      top: config.title ? 50 : 20,
      containLabel: true
    },
    xAxis: {
      type: 'value'
    },
    yAxis: {
      type: 'category',
      data: names
    },
    series: [
      {
        type: 'bar',
        data: values,
        itemStyle: {
          color: config.color,
          borderRadius: [0, 5, 5, 0]
        },
        label: config.showLabel ? {
          show: true,
          position: 'right'
        } : undefined
      }
    ]
  }
}

/**
 * 获取雷达图配置
 * @param {Object} data - 数据 {indicator: [{name: '', max: 0}], series: [{name: '', value: []}]}
 * @param {Object} options - 自定义配置
 */
export const getRadarChartOption = (data, options = {}) => {
  const defaultOptions = {
    title: '',
    shape: 'polygon'
  }
  
  const config = { ...defaultOptions, ...options }
  
  return {
    title: config.title ? {
      text: config.title,
      left: 'center',
      top: 10,
      textStyle: {
        fontSize: 16,
        fontWeight: 'bold'
      }
    } : undefined,
    tooltip: {
      trigger: 'item'
    },
    legend: {
      data: data.series.map(s => s.name),
      top: config.title ? 40 : 10
    },
    radar: {
      indicator: data.indicator,
      shape: config.shape
    },
    series: [
      {
        type: 'radar',
        data: data.series
      }
    ]
  }
}

/**
 * 获取仪表盘配置
 * @param {number} value - 数值
 * @param {Object} options - 自定义配置
 */
export const getGaugeChartOption = (value, options = {}) => {
  const defaultOptions = {
    title: '',
    min: 0,
    max: 100,
    unit: '%'
  }
  
  const config = { ...defaultOptions, ...options }
  
  return {
    title: config.title ? {
      text: config.title,
      left: 'center',
      top: 10,
      textStyle: {
        fontSize: 16,
        fontWeight: 'bold'
      }
    } : undefined,
    series: [
      {
        type: 'gauge',
        min: config.min,
        max: config.max,
        progress: {
          show: true,
          width: 18
        },
        axisLine: {
          lineStyle: {
            width: 18
          }
        },
        axisTick: {
          show: false
        },
        splitLine: {
          length: 15,
          lineStyle: {
            width: 2,
            color: '#999'
          }
        },
        axisLabel: {
          distance: 25,
          color: '#999',
          fontSize: 12
        },
        anchor: {
          show: true,
          showAbove: true,
          size: 25,
          itemStyle: {
            borderWidth: 10
          }
        },
        title: {
          show: false
        },
        detail: {
          valueAnimation: true,
          fontSize: 30,
          offsetCenter: [0, '70%'],
          formatter: `{value}${config.unit}`
        },
        data: [
          {
            value: value
          }
        ]
      }
    ]
  }
}

/**
 * 获取热力图配置
 * @param {Object} data - 数据 {xAxis: [], yAxis: [], data: [[x, y, value]]}
 * @param {Object} options - 自定义配置
 */
export const getHeatmapChartOption = (data, options = {}) => {
  const defaultOptions = {
    title: '',
    visualMapMin: 0,
    visualMapMax: 100
  }
  
  const config = { ...defaultOptions, ...options }
  
  return {
    title: config.title ? {
      text: config.title,
      left: 'center',
      top: 10,
      textStyle: {
        fontSize: 16,
        fontWeight: 'bold'
      }
    } : undefined,
    tooltip: {
      position: 'top'
    },
    grid: {
      height: '50%',
      top: config.title ? 80 : 50
    },
    xAxis: {
      type: 'category',
      data: data.xAxis,
      splitArea: {
        show: true
      }
    },
    yAxis: {
      type: 'category',
      data: data.yAxis,
      splitArea: {
        show: true
      }
    },
    visualMap: {
      min: config.visualMapMin,
      max: config.visualMapMax,
      calculable: true,
      orient: 'horizontal',
      left: 'center',
      bottom: '15%'
    },
    series: [
      {
        type: 'heatmap',
        data: data.data,
        label: {
          show: true
        },
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        }
      }
    ]
  }
}
