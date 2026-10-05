// Only register components and string icon props used by the admin UI.
import { ElAlert, ElAside, ElBreadcrumb, ElBreadcrumbItem, ElButton, ElCard, ElCascader, ElCol, ElContainer, ElDatePicker, ElDescriptions, ElDescriptionsItem, ElDialog, ElDivider, ElDropdown, ElDropdownItem, ElDropdownMenu, ElForm, ElFormItem, ElHeader, ElIcon, ElImage, ElInput, ElInputNumber, ElMain, ElMenu, ElMenuItem, ElOption, ElPagination, ElRadio, ElRadioButton, ElRadioGroup, ElRow, ElSelect, ElSubMenu, ElSwitch, ElTabPane, ElTable, ElTableColumn, ElTabs, ElTag, ElUpload, ElLoading } from 'element-plus'
import { Delete, Download, Expand, Fold, Lock, Plus, Refresh, Search, User } from '@element-plus/icons-vue'
import 'element-plus/dist/index.css'

export function installUi(app) {
  const components = { ElAlert, ElAside, ElBreadcrumb, ElBreadcrumbItem, ElButton, ElCard, ElCascader, ElCol, ElContainer, ElDatePicker, ElDescriptions, ElDescriptionsItem, ElDialog, ElDivider, ElDropdown, ElDropdownItem, ElDropdownMenu, ElForm, ElFormItem, ElHeader, ElIcon, ElImage, ElInput, ElInputNumber, ElMain, ElMenu, ElMenuItem, ElOption, ElPagination, ElRadio, ElRadioButton, ElRadioGroup, ElRow, ElSelect, ElSubMenu, ElSwitch, ElTabPane, ElTable, ElTableColumn, ElTabs, ElTag, ElUpload, Delete, Download, Expand, Fold, Lock, Plus, Refresh, Search, User }
  for (const [name, component] of Object.entries(components)) app.component(name, component)
  app.directive('loading', ElLoading.directive)
}
