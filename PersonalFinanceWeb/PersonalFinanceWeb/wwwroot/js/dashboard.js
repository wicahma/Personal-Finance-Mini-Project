let topCategoriesChart = null;

window.renderDashboardChart = function (labels, data, backgroundColors) {
  const ctx = document.getElementById("topCategoriesChart");
  if (!ctx) return;

  if (topCategoriesChart) {
    topCategoriesChart.destroy();
    topCategoriesChart = null;
  }

  topCategoriesChart = new Chart(ctx, {
    type: "bar",
    data: {
      labels: labels,
      datasets: [
        {
          label: "Spending",
          data: data,
          backgroundColor: backgroundColors,
          borderColor: "#F0F0F0",
          borderWidth: 2,
          borderRadius: 0,
          barPercentage: 0.7,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      animation: { duration: 400 },
      plugins: {
        legend: { display: false },
        tooltip: {
          backgroundColor: "#141414",
          borderColor: "#F0F0F0",
          borderWidth: 2,
          titleColor: "#F0F0F0",
          bodyColor: "#F0F0F0",
          padding: 12,
          callbacks: {
            label: (ctx) =>
              ` $${ctx.parsed.y.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`,
          },
        },
      },
      scales: {
        x: {
          grid: { display: false },
          border: { color: "#2A2A2A", width: 2 },
          ticks: {
            color: "#F0F0F0",
            font: { weight: "700", size: 12 },
            maxRotation: 30,
          },
        },
        y: {
          grid: {
            color: "#2A2A2A",
            lineWidth: 1,
          },
          border: { color: "#2A2A2A", width: 2 },
          ticks: {
            color: "#F0F0F0",
            font: { weight: "600", size: 11 },
            callback: (val) => "$" + val.toLocaleString(),
          },
        },
      },
    },
  });
};

window.destroyDashboardChart = function () {
  if (topCategoriesChart) {
    topCategoriesChart.destroy();
    topCategoriesChart = null;
  }
};
