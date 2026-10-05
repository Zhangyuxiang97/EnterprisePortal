<template>
  <dialog ref="dialog" aria-label="图片预览" class="m-auto max-h-[95dvh] max-w-[95vw] bg-transparent p-0" @close="$emit('close')" @click="dialog.close()">
    <button type="button" aria-label="关闭图片预览" class="fixed right-6 top-6 rounded-full bg-black/60 p-3 text-2xl text-white" @click="dialog.close()">×</button>
    <img :src="src" alt="预览图片" @click.stop class="max-h-[90dvh] max-w-full object-contain" />
  </dialog>
</template>
<script setup>
import { ref, onMounted } from 'vue'
defineProps({ src: String })
defineEmits(['close'])
const dialog = ref(null)
onMounted(() => dialog.value.showModal())
</script>
<style scoped>
dialog::backdrop { background: rgb(0 0 0 / .85); }
</style>
