/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    // Server project
    "./Components/**/*.{razor,html,cs}",
    "./Pages/**/*.{razor,html,cs}",
    // WASM client project
    "../PersonalFinanceWeb.Client/Components/**/*.{razor,html,cs}",
    "../PersonalFinanceWeb.Client/Layout/**/*.{razor,html,cs}",
    "../PersonalFinanceWeb.Client/Pages/**/*.{razor,html,cs}",
    "../PersonalFinanceWeb.Client/*.razor",
  ],
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        "base-bg": "#0A0A0A",
        "base-surface": "#141414",
        "base-border": "#2A2A2A",
        neon: "#39FF14",
        "vivid-yellow": "#FFE600",
        "hot-pink": "#FF2D78",
        "soft-white": "#F0F0F0",
      },
      boxShadow: {
        "brutal-green": "4px 4px 0px #39FF14",
        "brutal-yellow": "4px 4px 0px #FFE600",
        "brutal-pink": "4px 4px 0px #FF2D78",
        "brutal-white": "4px 4px 0px #F0F0F0",
        "brutal-green-lg": "6px 6px 0px #39FF14",
        "brutal-yellow-lg": "6px 6px 0px #FFE600",
        "brutal-pink-lg": "6px 6px 0px #FF2D78",
      },
      fontFamily: {
        sans: ["Inter", "ui-sans-serif", "system-ui", "sans-serif"],
      },
    },
  },
  plugins: [],
};
