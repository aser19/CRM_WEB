using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace BiztvillCRM.Services;

/// <summary>
/// Segédosztály meglévő PDF dokumentumok összefűzéséhez (pl. egy generált jegyzőkönyv
/// végére egy feltöltött PDF formátumú bizonyítvány csatolásához).
/// </summary>
public static class PdfMergeHelper
{
    /// <summary>
    /// Hozzáfűzi a <paramref name="csatolandoPdfek"/> listában szereplő PDF-eket a <paramref name="foDokumentum"/> végéhez.
    /// A csatolt oldalak változatlan formában (saját fejléc/lábléc/oldalszám nélkül) kerülnek be.
    /// Ha egy csatolandó PDF érvénytelen vagy nem olvasható be, az adott fájlt kihagyja, a többit feldolgozza.
    /// </summary>
    /// <param name="foDokumentum">A fő (pl. QuestPDF-fel generált) PDF tartalma.</param>
    /// <param name="csatolandoPdfek">A végére fűzendő PDF fájlok tartalma (ebben a sorrendben).</param>
    /// <returns>Az összefűzött PDF bájtjai.</returns>
    public static byte[] Fuzzel(byte[] foDokumentum, IEnumerable<byte[]> csatolandoPdfek)
    {
        using var output = new PdfDocument();

        using (var ms = new MemoryStream(foDokumentum))
        using (var foDoc = PdfReader.Open(ms, PdfDocumentOpenMode.Import))
        {
            foreach (var page in foDoc.Pages)
                output.AddPage(page);
        }

        foreach (var pdfBytes in csatolandoPdfek)
        {
            if (pdfBytes is null || pdfBytes.Length == 0)
                continue;

            try
            {
                using var ms = new MemoryStream(pdfBytes);
                using var doc = PdfReader.Open(ms, PdfDocumentOpenMode.Import);
                foreach (var page in doc.Pages)
                    output.AddPage(page);
            }
            catch
            {
                // Érvénytelen vagy sérült/jelszóval védett PDF - kihagyjuk, a többi feldolgozása folytatódik.
            }
        }

        using var resultStream = new MemoryStream();
        output.Save(resultStream);
        return resultStream.ToArray();
    }
}
