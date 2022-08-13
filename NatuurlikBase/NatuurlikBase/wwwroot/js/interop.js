
function createAlert() {
    alert('Hello');
}


function downloadPDF() {    
    var header = document.querySelector('#head');
    var doc = new jsPDF();
    //HEADER
    var width = doc.internal.pageSize.getWidth();
    var height = doc.internal.pageSize.getHeight();
    doc.addImage(header, 'png', 0, 8, width, height - 250)
    //TITLE, DATE, PRINTED BY AND COUNT
    doc.setFontSize(18);
    doc.text(13, 60, "PRODUCTION REPORT");
    doc.setFontSize(8);
    var date = new Date().toLocaleDateString();
    var printedby = "Kyle van Eeden";
    doc.text(13, 66, "PRINTED BY: " + printedby);
    doc.text(160, 66, "DATE PRINTED: " + date);
    
    //TABLE 
    doc.autoTable( {html: '#tblData', startY:82, headStyles: {fillColor: [35, 35, 35]} });

    //FOOTER, NEED TO ADD PAGE NUMBERS
    doc.setFontSize(10);
    let finalY = doc.autoTable.previous.finalY;
    doc.text(88, finalY+20, "***END OF REPORT***");    
        doc.save('NatuurlikProductionReport.pdf');
    }
