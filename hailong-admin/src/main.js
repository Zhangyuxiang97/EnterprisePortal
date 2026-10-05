import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { installUi } from './plugins-ui'
import App from './App.vue'
import router from './router'
import './style.css'

const app = createApp(App)
const pinia = createPinia()

installUi(app)

app.use(pinia)
app.use(router)

app.mount('#app')
