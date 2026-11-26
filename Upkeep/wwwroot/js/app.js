window.upkeep = {
    downloadFile(filename, content) {
        const blob = new Blob([content], { type: "application/json;charset=utf-8" });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = url;
        link.download = filename;
        link.click();
        window.setTimeout(() => URL.revokeObjectURL(url), 1000);
    },
    clearFileInput(id) {
        const input = document.getElementById(id);
        if (input) {
            input.value = "";
        }
    }
};
