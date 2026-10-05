import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
import { loadSiteConfig } from './utils/config'

loadSiteConfig()
createApp(App).use(router).mount('#app')
