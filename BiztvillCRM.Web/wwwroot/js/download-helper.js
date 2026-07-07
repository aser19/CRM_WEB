// Fájl letöltés helper függvény Base64 adatból
export function downloadFile(fileName, contentType, base64Data) {
    try {
        // Base64 dekódolás
        const byteCharacters = atob(base64Data);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);

        // Blob létrehozása
        const blob = new Blob([byteArray], { type: contentType });

        // Link létrehozása és kattintás szimulálása
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        // URL felszabadítása
        window.URL.revokeObjectURL(link.href);

        return true;
    } catch (error) {
        console.error('Letöltési hiba:', error);
        return false;
    }
}

// Globális scope-ba helyezés, ha nem modul
if (typeof window !== 'undefined') {
    window.downloadFile = downloadFile;
}
