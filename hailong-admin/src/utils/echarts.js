import { use, init, graphic } from 'echarts/core'
import { LineChart, BarChart, PieChart, GaugeChart } from 'echarts/charts'
import { GridComponent, LegendComponent, TooltipComponent, TitleComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'

use([LineChart, BarChart, PieChart, GaugeChart, GridComponent, LegendComponent, TooltipComponent, TitleComponent, CanvasRenderer])
export { init, graphic }
