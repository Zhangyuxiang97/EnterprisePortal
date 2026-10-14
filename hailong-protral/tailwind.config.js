/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        hailong: {
          primary: '#245B85',
          secondary: '#64748B',
          dark: '#0F1419',
          cyan: '#4E8290'
        }
      },
      fontFamily: {
        'songti': ['"SimSun"', '"STSong"', 'serif'],
        'yahei': ['"Microsoft YaHei"', '"微软雅黑"', 'sans-serif'],
        'tech': ['"SF Pro Display"', '"Helvetica Neue"', 'Arial', 'sans-serif']
      },
      animation: {
        'fade-in': 'fadeIn 0.6s ease-out',
        'slide-up': 'slideUp 0.8s ease-out',
        'float': 'float 3s ease-in-out infinite'
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' }
        },
        slideUp: {
          '0%': { transform: 'translateY(30px)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' }
        },
        float: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(-10px)' }
        }
      }
    },
  },
  plugins: [],
}
