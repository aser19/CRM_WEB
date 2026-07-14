window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    try {
        console.log('downloadFileFromStream called with:', fileName);

        if (!contentStreamReference) {
            throw new Error('contentStreamReference is null or undefined');
        }

        const arrayBuffer = await contentStreamReference.arrayBuffer();

        if (!arrayBuffer || arrayBuffer.byteLength === 0) {
            throw new Error('Array buffer is empty');
        }

        const mimeTypes = {
            '.pdf': 'application/pdf',
            '.docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
            '.xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        };

        const ext = fileName.substring(fileName.lastIndexOf('.'));
        const mimeType = mimeTypes[ext] || 'application/octet-stream';

        const blob = new Blob([arrayBuffer], { type: mimeType });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);

        // Kis késleltetés után törölni a URL-t
        setTimeout(() => {
            URL.revokeObjectURL(url);
        }, 100);

        console.log('File download initiated successfully:', fileName);
    } catch (error) {
        console.error('Error in downloadFileFromStream:', error);
        alert('Hiba a fájl letöltése során: ' + error.message);
        throw error;
    }
};

window.downloadFileFromBytes = (fileName, mimeType, bytes) => {
    const uint8Array = new Uint8Array(bytes);
    const blob = new Blob([uint8Array], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

window.downloadFile = (fileName, base64Content) => {
    const mimeTypes = {
        '.pdf': 'application/pdf',
        '.docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
        '.xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    };

    const ext = fileName.substring(fileName.lastIndexOf('.'));
    const mimeType = mimeTypes[ext] || 'application/octet-stream';

    const byteCharacters = atob(base64Content);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

window.downloadFileFromByteArray = (fileName, mimeType, byteArray) => {
    const blob = new Blob([byteArray], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

// Jelezzük, hogy a download.js betöltődött
console.log('download.js loaded successfully');
console.log('downloadFileFromStream available:', typeof window.downloadFileFromStream === 'function');
};