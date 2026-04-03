// PDF és egyéb fájlok letöltése Blazor Server-ből
window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName ?? 'download.pdf';
    anchor.click();
    
    URL.revokeObjectURL(url);
};

// Alternatív: byte tömb közvetlen letöltése
window.downloadFileFromBytes = (fileName, contentType, byteArray) => {
    const blob = new Blob([new Uint8Array(byteArray)], { type: contentType });
    const url = URL.createObjectURL(blob);
    
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName;
    anchor.click();
    
    URL.revokeObjectURL(url);
};