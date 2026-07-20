<template>
  <div class="min-h-screen bg-slate-50/50">
    <!-- 导航栏 -->
    <Header />

    <!-- 页面头部 Hero Banner -->
    <div class="relative pt-32 pb-20 text-center text-white overflow-hidden bg-gradient-to-br from-hailong-dark via-slate-900 to-indigo-950">
      <div class="absolute inset-0 bg-grid-pattern opacity-10 pointer-events-none"></div>
      <div class="absolute -top-40 left-1/2 -translate-x-1/2 w-[600px] h-[600px] bg-hailong-primary/10 rounded-full blur-[120px] pointer-events-none animate-float"></div>
      
      <div class="relative z-10 max-w-4xl mx-auto px-6">
        <h1 class="text-4xl md:text-5xl font-extrabold mb-4 font-tech tracking-tight bg-gradient-to-r from-white via-slate-100 to-slate-200 bg-clip-text text-transparent">
          专家库
        </h1>
        <p class="text-base md:text-lg text-slate-300 font-medium max-w-2xl mx-auto">
          诚邀各领域专家加入海隆咨询专家库，携手共创专业工程咨询生态
        </p>
      </div>
    </div>

    <!-- 主要内容区域 -->
    <div class="py-10">
      <div class="container-wide max-w-4xl mx-auto">
        <!-- 主要内容卡片 -->
        <div class="bg-white rounded-2xl shadow-[0_8px_30px_rgb(0,0,0,0.02)] overflow-hidden border border-slate-100 animate-fade-in">
          <div class="p-8 md:p-12">
            <!-- 二维码和公告内容区域 -->
            <div class="mb-8">
              <!-- 移动端：显示二维码和填写按钮 -->
              <div class="flex flex-col items-center md:hidden mb-8 space-y-4">
                <div class="bg-gradient-to-br from-hailong-primary/10 to-hailong-secondary/10 p-6 rounded-xl border-2 border-hailong-cyan/30">
                  <div class="bg-white rounded-lg p-3">
                    <img :src="qrCodeImage" alt="专家库二维码" class="w-48 h-48 object-contain" />
                    <p class="text-xs text-gray-600 mt-2 text-center font-medium">扫码填写信息</p>
                  </div>
                </div>
                <!-- 移动端填写按钮 -->
                <button
                  @click="showFormAndScroll"
                  class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white font-semibold rounded-lg hover:shadow-xl transform hover:-translate-y-0.5 transition-all duration-200 w-full max-w-xs justify-center"
                >
                  <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
                  </svg>
                  在线填写
                  <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
                  </svg>
                </button>
              </div>

              <!-- 电脑端：公告内容（包含二维码） -->
              <div class="hidden md:block">
                <div class="bg-gradient-to-r from-hailong-primary/5 to-hailong-secondary/5 p-6 md:p-8 rounded-xl border-l-4 border-hailong-primary">
                  <div class="flex gap-6">
                    <!-- 文字内容 -->
                    <div class="flex-1">
                      <p class="text-gray-700 leading-relaxed mb-4">
                        海隆工程咨询有限公司现已启动代理专家库信息录入工作，特此诚邀您参与。
                      </p>
                      <p class="text-gray-700 leading-relaxed mb-4">
                        为便于后续项目合作，请您认真、如实填写相关信息。我们将在资料审核通过后，将您的信息正式录入专家库抽取系统。
                      </p>
                      <p class="text-gray-700 leading-relaxed font-medium mb-6">
                        感谢您的支持与配合！
                      </p>
                      
                      <!-- 电脑端：填写按钮组 -->
                      <div class="flex gap-4 mt-6">
                        <button
                          @click="showFormAndScroll"
                          class="inline-flex items-center px-8 py-3 bg-gradient-to-r from-hailong-primary to-hailong-secondary text-white font-semibold rounded-lg hover:shadow-xl transform hover:-translate-y-0.5 transition-all duration-200"
                        >
                          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
                          </svg>
                          电脑端填写
                          <svg class="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
                          </svg>
                        </button>
                        <button
                          @click="toggleQRCode"
                          class="inline-flex items-center px-8 py-3 bg-white text-hailong-primary border-2 border-hailong-primary font-semibold rounded-lg hover:bg-hailong-primary hover:text-white hover:shadow-xl transform hover:-translate-y-0.5 transition-all duration-200"
                        >
                          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                          </svg>
                          {{ showQRCode ? '隐藏二维码' : '手机端填写' }}
                        </button>
                      </div>
                    </div>

                    <!-- 二维码区域（可切换显示，在公告框内右侧） -->
                    <div v-if="showQRCode" class="flex-shrink-0 flex items-center">
                      <div class="bg-white rounded-lg p-3 shadow-md">
                        <img :src="qrCodeImage" alt="专家库二维码" class="w-48 h-48 object-contain" />
                        <p class="text-xs text-gray-600 mt-2 text-center font-medium">扫码填写</p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>


            <!-- 分隔线 -->
            <div class="border-t border-gray-200 my-8"></div>

            <!-- 联系方式区域 -->
            <div class="bg-gray-50 p-6 rounded-xl border border-gray-200">
              <h3 class="text-xl font-bold text-hailong-dark mb-4 flex items-center">
                <svg class="w-6 h-6 mr-2 text-hailong-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"></path>
                </svg>
                联系方式
              </h3>
              <div class="space-y-3">
                <div class="flex items-center text-gray-800">
                  <span class="font-medium text-gray-900 mr-2">联系电话：</span>
                  <a href="tel:0371-55894666" class="text-hailong-primary hover:text-hailong-secondary transition-colors font-semibold text-lg">
                    {{ companyInfo.contact.phone }}
                  </a>
                </div>
                <div class="flex items-center text-gray-800">
                  <span class="font-medium text-gray-900 mr-2">联系邮箱：</span>
                  <a :href="`mailto:${companyInfo.contact.email}`" class="text-hailong-primary hover:text-hailong-secondary transition-colors">
                    {{ companyInfo.contact.email }}
                  </a>
                </div>
                <div class="flex items-start text-gray-800">
                  <span class="font-medium text-gray-900 mr-2 flex-shrink-0">办公地址：</span>
                  <span>{{ companyInfo.contact.address.fullAddress }}</span>
                </div>
              </div>
            </div>

            <!-- 温馨提示 -->
            <div class="bg-blue-50 border border-blue-200 rounded-lg p-4 mt-8">
              <div class="flex items-start">
                <svg class="w-5 h-5 text-blue-500 mr-2 flex-shrink-0 mt-0.5" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"></path>
                </svg>
                <div class="text-sm text-blue-800">
                  <p class="font-medium mb-1">温馨提示：</p>
                  <ul class="list-disc list-inside space-y-1 text-blue-700">
                    <li class="md:hidden">可点击"在线填写"按钮直接填写，或使用微信扫描二维码填写</li>
                    <li class="hidden md:list-item">推荐使用"电脑端填写"按钮在弹窗中直接填写</li>
                    <li class="hidden md:list-item">如不方便使用电脑，可点击"手机端填写"按钮显示二维码，使用手机扫码填写</li>
                    <li>请确保填写信息真实、准确、完整</li>
                    <li>如有疑问，请拨打联系电话咨询</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 页脚 -->
    <Footer />

    <!-- 表单弹窗（支持移动端和PC端） -->
    <div v-if="showForm"
      class="fixed inset-0 z-[100] bg-black/50 animate-fade-in flex items-center justify-center p-0 md:p-4"
      @click.self="hideForm">
      <div class="bg-white rounded-none md:rounded-2xl shadow-2xl w-full h-full md:h-[85vh] md:max-w-4xl flex flex-col animate-scale-in">
        <!-- 标题栏 -->
        <div class="bg-gradient-to-r from-hailong-primary to-hailong-secondary p-3 md:p-4 flex items-center justify-between shadow-lg rounded-t-none md:rounded-t-2xl">
          <h3 class="text-lg md:text-xl font-bold text-white flex items-center">
            <svg class="w-5 h-5 md:w-6 md:h-6 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"></path>
            </svg>
            填写专家信息
          </h3>
          <button
            @click="hideForm"
            class="text-white hover:text-gray-200 transition-colors p-1.5 md:p-2 hover:bg-white/10 rounded-lg"
            title="关闭表单">
            <svg class="w-5 h-5 md:w-6 md:h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>
        <!-- iframe 内容区域 -->
        <div class="flex-1 overflow-hidden rounded-b-none md:rounded-b-2xl">
          <iframe
            src="https://www.wjx.cn/vm/tkPK2g4.aspx"
            class="w-full h-full border-0"
            frameborder="0"
            scrolling="yes"
            allowfullscreen
            sandbox="allow-same-origin allow-scripts allow-forms allow-popups allow-popups-to-escape-sandbox"
          ></iframe>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import Header from '@/components/Header.vue'
import Footer from '@/components/Footer.vue'
import { getCompanyInfo } from '@/utils/config'
import qrCodeImage from '@/assets/QR_CODE.png'

// 表单显示状态
const showForm = ref(false)
// 二维码显示状态
const showQRCode = ref(false)

// 显示表单
const showFormAndScroll = () => {
  showForm.value = true
}

// 隐藏表单
const hideForm = () => {
  showForm.value = false
}

// 切换二维码显示
const toggleQRCode = () => {
  showQRCode.value = !showQRCode.value
}

// 监听表单显示状态，控制背景滚动
watch(showForm, (newValue) => {
  if (newValue) {
    // 显示表单时，禁止背景滚动
    document.body.style.overflow = 'hidden'
  } else {
    // 隐藏表单时，恢复背景滚动
    document.body.style.overflow = ''
  }
})

// 获取公司信息
const companyInfo = computed(() => ({
  contact: getCompanyInfo().contact || {
    phone: '0371-55894666',
    email: '037155894666@henanhailong.com',
    address: {
      fullAddress: '郑州市郑东新区金水东路85号雅宝·东方国际广场2号楼13层'
    }
  }
}))

// 页面加载时滚动到顶部
onMounted(() => {
  window.scrollTo(0, 0)
})

// 组件卸载时恢复滚动
onUnmounted(() => {
  document.body.style.overflow = ''
})
</script>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.3s ease-in;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-scale-in {
  animation: scaleIn 0.3s ease-out;
}

@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.prose {
  color: #374151;
}

.prose p {
  margin-bottom: 0;
}

.bg-grid-pattern {
  background-image: linear-gradient(to right, rgba(255,255,255,0.05) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 24px 24px;
}
</style>