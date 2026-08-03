document.querySelectorAll('.lc-link').forEach(link => {

    link.addEventListener('click', function () {

        const status = this.dataset.status;

        toggleAllSections(status);
    });

});

function getActiveTable(btn) {

    // Find current active tab
    const activeTab = btn.closest('.tab-pane');

    if (!activeTab) {
        alert('Active tab not found');
        return null;
    }

    // Find visible table only
    const tables = activeTab.querySelectorAll('table');

    for (let table of tables) {

        const parentDiv = table.closest('div');

        if (
            table.offsetParent !== null &&
            (!parentDiv || window.getComputedStyle(parentDiv).display !== 'none')
        ) {
            return table;
        }
    }

    alert('No visible table found');
    return null;
}

/* print table pdf */

function downloadCurrentPDF(btn) {

    const table = getActiveTable(btn);

    if (!table) return;

    html2pdf()
        .set({
            margin: 0.3,
            filename: 'LC_Report.pdf',
            image: {
                type: 'jpeg',
                quality: 1
            },
            html2canvas: {
                scale: 2
            },
            jsPDF: {
                unit: 'in',
                format: 'a3',
                orientation: 'landscape'
            }
        })
        .from(table)
        .save();
}


/* print table excel */

function downloadCurrentExcel(btn) {

    const table = getActiveTable(btn);

    if (!table) return;

    const workbook = XLSX.utils.table_to_book(table, {
        sheet: "Report"
    });

    XLSX.writeFile(workbook, "LC_Report.xlsx");
}

/* print table */

function printCurrentTable(btn) {

    const table = getActiveTable(btn);

    if (!table) return;

    const printWindow = window.open('', '', 'width=1400,height=900');

    printWindow.document.write(`
        <html>
        <head>
            <title>LC Report</title>
            <style>
                body{
                    font-family:Arial;
                    padding:20px;
                }

                table{
                    width:100%;
                    border-collapse:collapse;
                }

                th,td{
                    border:1px solid #000;
                    padding:8px;
                    font-size:12px;
                }

                th{
                    background:#f2f2f2;
                }
            </style>
        </head>
        <body>
            <h2>LC Report</h2>
            ${table.outerHTML}
        </body>
        </html>
    `);

    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
}


function toggleTenderData() {

    const selectedRadio = document.querySelector('input[name="status"]:checked');

    if (!selectedRadio) return;

    const selectedValue = selectedRadio.value;

    const sanctionDiv = document.getElementById('tendersanctionDiv');
    const unsanctionDiv = document.getElementById('tenderunsanctionDiv');

    if (selectedValue === 'sanction') {

        sanctionDiv.style.display = 'block';
        unsanctionDiv.style.display = 'none';

    } else {

        sanctionDiv.style.display = 'none';
        unsanctionDiv.style.display = 'block';

    }
}


function toggleCAPlanData() {

    const selectedRadio =
        document.querySelector('input[name="caStatus"]:checked');

    if (!selectedRadio) return;

    const selectedValue = selectedRadio.value;

    const sanctionDiv =
        document.getElementById('caPlansanctionDiv');

    const unsanctionDiv =
        document.getElementById('caPlanunsanctionDiv');

    if (selectedValue === 'sanction') {

        sanctionDiv.style.display = 'block';
        unsanctionDiv.style.display = 'none';

    } else {

        sanctionDiv.style.display = 'none';
        unsanctionDiv.style.display = 'block';

    }
}


function toggleDesignData() {

    const selectedRadio =
        document.querySelector('input[name="designStatus"]:checked');

    if (!selectedRadio) return;

    const selectedValue = selectedRadio.value;

    const sanctionDiv =
        document.getElementById('designSanctionDiv');

    const unsanctionDiv =
        document.getElementById('designUnSanctionDiv');

    if (selectedValue === 'sanction') {

        sanctionDiv.style.display = 'block';
        unsanctionDiv.style.display = 'none';

    } else {

        sanctionDiv.style.display = 'none';
        unsanctionDiv.style.display = 'block';
    }
}

function toggleLHSData() {

    const selectedRadio =
        document.querySelector('input[name="lhsStatus"]:checked');

    if (!selectedRadio) return;

    const selectedValue = selectedRadio.value;

    if (selectedValue === 'sanction') {

        document.getElementById('lhsSanctionDiv').style.display = 'block';
        document.getElementById('lhsUnSanctionDiv').style.display = 'none';

    } else {

        document.getElementById('lhsSanctionDiv').style.display = 'none';
        document.getElementById('lhsUnSanctionDiv').style.display = 'block';
    }
}

function toggleNOCData() {

    const selectedRadio =
        document.querySelector('input[name="nocStatus"]:checked');

    if (!selectedRadio) return;

    if (selectedRadio.value === 'sanction') {

        document.getElementById('nocSanctionDiv').style.display = 'block';
        document.getElementById('nocUnSanctionDiv').style.display = 'none';

    } else {

        document.getElementById('nocSanctionDiv').style.display = 'none';
        document.getElementById('nocUnSanctionDiv').style.display = 'block';
    }
}


window.addEventListener('load', function () {

    toggleTenderData();
    toggleCAPlanData();
    toggleDesignData();
    toggleLHSData();

});

function toggleSUR(type) {

    const sanctionDiv = document.getElementById('surSanctionDiv');
    const unSanctionDiv = document.getElementById('surUnSanctionDiv');

    const sanctionBtn = document.getElementById('surSanctionBtn');
    const unSanctionBtn = document.getElementById('surUnSanctionBtn');

    if (type === 'sanction') {

        sanctionDiv.style.display = 'block';
        unSanctionDiv.style.display = 'none';

        sanctionBtn.className = 'btn btn-success btn-sm';
        unSanctionBtn.className = 'btn btn-outline-danger btn-sm';

    } else {

        sanctionDiv.style.display = 'none';
        unSanctionDiv.style.display = 'block';

        unSanctionBtn.className = 'btn btn-danger btn-sm';
        sanctionBtn.className = 'btn btn-outline-success btn-sm';
    }
}


function toggleAllSections(status) {

    const sanctionSections = [
        'sanctionDiv',
        'tendersanctionDiv',
        'caPlansanctionDiv',
        'designSanctionDiv',
        'lhsSanctionDiv',
        'nocSanctionDiv'
    ];

    const unSanctionSections = [
        'unsanctionDiv',
        'tenderunsanctionDiv',
        'caPlanunsanctionDiv',
        'designUnSanctionDiv',
        'lhsUnSanctionDiv',
        'nocUnSanctionDiv'
    ];

    if (status === 'sanction') {

        sanctionSections.forEach(id => {
            document.getElementById(id).style.display = 'block';
        });

        unSanctionSections.forEach(id => {
            document.getElementById(id).style.display = 'none';
        });

    } else {

        sanctionSections.forEach(id => {
            document.getElementById(id).style.display = 'none';
        });

        unSanctionSections.forEach(id => {
            document.getElementById(id).style.display = 'block';
        });
    }
}

function togglePuneData(type) {

    const sanctionDiv =
        document.getElementById('puneSanctionDiv');

    const unSanctionDiv =
        document.getElementById('puneUnSanctionDiv');

    const sanctionBtn =
        document.getElementById('puneSanctionBtn');

    const unSanctionBtn =
        document.getElementById('puneUnSanctionBtn');

    if (type === 'sanction') {

        sanctionDiv.style.display = 'block';
        unSanctionDiv.style.display = 'none';

        sanctionBtn.className =
            'btn btn-success btn-sm';

        unSanctionBtn.className =
            'btn btn-outline-danger btn-sm';

    } else {

        sanctionDiv.style.display = 'none';
        unSanctionDiv.style.display = 'block';

        sanctionBtn.className =
            'btn btn-outline-success btn-sm';

        unSanctionBtn.className =
            'btn btn-danger btn-sm';
    }
}


function toggleMumbaiData(type) {

    const sanctionDiv =
        document.getElementById('mumbaiSanctionDiv');

    const unSanctionDiv =
        document.getElementById('mumbaiUnSanctionDiv');

    const sanctionBtn =
        document.getElementById('mumbaiSanctionBtn');

    const unSanctionBtn =
        document.getElementById('mumbaiUnSanctionBtn');

    if (type === 'sanction') {

        sanctionDiv.style.display = 'block';
        unSanctionDiv.style.display = 'none';

        sanctionBtn.className =
            'btn btn-success btn-sm';

        unSanctionBtn.className =
            'btn btn-outline-danger btn-sm';

    } else {

        sanctionDiv.style.display = 'none';
        unSanctionDiv.style.display = 'block';

        sanctionBtn.className =
            'btn btn-outline-success btn-sm';

        unSanctionBtn.className =
            'btn btn-danger btn-sm';
    }
}


function toggleNagpurData(type) {

    const sanctionDiv =
        document.getElementById('nagpurSanctionDiv');

    const unSanctionDiv =
        document.getElementById('nagpurUnSanctionDiv');

    const sanctionBtn =
        document.getElementById('nagpurSanctionBtn');

    const unSanctionBtn =
        document.getElementById('nagpurUnSanctionBtn');

    if (type === 'sanction') {

        sanctionDiv.style.display = 'block';
        unSanctionDiv.style.display = 'none';

        sanctionBtn.className =
            'btn btn-success btn-sm';

        unSanctionBtn.className =
            'btn btn-outline-danger btn-sm';

    } else {

        sanctionDiv.style.display = 'none';
        unSanctionDiv.style.display = 'block';

        sanctionBtn.className =
            'btn btn-outline-success btn-sm';

        unSanctionBtn.className =
            'btn btn-danger btn-sm';
    }
}

function toggleBhusawalData(type) {

    const sanctionDiv =
        document.getElementById('bhusawalSanctionDiv');

    const unSanctionDiv =
        document.getElementById('bhusawalUnSanctionDiv');

    const sanctionBtn =
        document.getElementById('bhusawalSanctionBtn');

    const unSanctionBtn =
        document.getElementById('bhusawalUnSanctionBtn');

    if (type === 'sanction') {

        sanctionDiv.style.display = 'block';
        unSanctionDiv.style.display = 'none';

        sanctionBtn.className =
            'btn btn-success btn-sm';

        unSanctionBtn.className =
            'btn btn-outline-danger btn-sm';

    } else {

        sanctionDiv.style.display = 'none';
        unSanctionDiv.style.display = 'block';

        sanctionBtn.className =
            'btn btn-outline-success btn-sm';

        unSanctionBtn.className =
            'btn btn-danger btn-sm';
    }
}

/* download table generic */

function downloadPDF(btn) {

    const tableId = btn.dataset.table;
    const fileName = btn.dataset.file;

    const element = document.getElementById(tableId);

    if (!element) {
        alert('Element not found');
        return;
    }

    const options = {
        margin: [0.3, 0.3, 0.3, 0.3],
        filename: fileName + '.pdf',
        image: {
            type: 'jpeg',
            quality: 1
        },
        html2canvas: {
            scale: 3,
            useCORS: true,
            scrollY: 0
        },
        jsPDF: {
            unit: 'mm',
            format: 'a3',
            orientation: 'landscape'
        },
        pagebreak: {
            mode: ['avoid-all', 'css', 'legacy']
        }
    };

    html2pdf()
        .set(options)
        .from(element)
        .save();
}


function downloadExcel(btn) {

    const tableId = btn.dataset.table;
    const fileName = btn.dataset.file;

    const table = document.getElementById(tableId);

    if (!table) {
        alert('Table not found');
        return;
    }

    const workbook =
        XLSX.utils.table_to_book(table, {
            sheet: 'Report'
        });

    XLSX.writeFile(workbook, fileName + '.xlsx');
}


function printTable(btn) {

    const tableId = btn.dataset.table;
    const title = btn.dataset.title;

    const table = document.getElementById(tableId);

    if (!table) {
        alert('Table not found');
        return;
    }

    const win =
        window.open('', '', 'width=1400,height=900');

    win.document.write(`
        <html>
        <head>
            <title>${title}</title>

            <style>
                body{
                    font-family:Arial,sans-serif;
                    padding:20px;
                }

                h2{
                    text-align:center;
                    margin-bottom:20px;
                }

                table{
                    width:100%;
                    border-collapse:collapse;
                }

                th,td{
                    border:1px solid #000;
                    padding:8px;
                }

                th{
                    background:#f2f2f2;
                }
            </style>

        </head>

        <body>

            <h2>${title}</h2>

            ${table.outerHTML}

        </body>
        </html>
    `);

    win.document.close();

    setTimeout(() => {
        win.print();
        win.close();
    }, 500);
}

/* download table generic */

/* edit row */

const STORAGE_KEY = "designTableData";

function editRow(link) {

    const row = link.closest("tr");

    const table = row.closest(".editable-table");

    if (link.innerText.trim() === "Edit") {

        link.innerText = "Save";

        row.querySelectorAll("td").forEach((cell, index) => {

            if (index === 0) return;

            if (cell.querySelector(".lc-link")) return;

            if (cell.hasAttribute("rowspan")) return;

            const value = cell.innerHTML.replace(/<br\s*\/?>/gi, "\n");

            cell.innerHTML =
                `<textarea class="form-control form-control-sm"
                    rows="2">${value}</textarea>`;

        });

    }
    else {
        link.innerText = "Edit";
        row.querySelectorAll("textarea").forEach(txt => {
            txt.parentElement.innerHTML =
                txt.value.replace(/\n/g, "<br>");
        });
        saveTable(table);
    }
}

function saveTable(table) {
    const storageKey =
        "TABLE_" + table.dataset.table;
    const data = [];
    table.querySelectorAll("tbody tr").forEach((row, r) => {
        row.querySelectorAll("td").forEach((cell, c) => {
            data.push({
                row: r,
                col: c,
                value: cell.innerHTML
            });
        });
    });

    localStorage.setItem(
        storageKey,
        JSON.stringify(data)
    );

}

function loadTables() {

    document.querySelectorAll(".editable-table")
        .forEach(table => {
            const storageKey =
                "TABLE_" + table.dataset.table;
            const saved =
                localStorage.getItem(storageKey);
            if (!saved)
                return;
            const data =
                JSON.parse(saved);
            data.forEach(item => {
                const row =
                    table.tBodies[0].rows[item.row];
                if (!row)
                    return;
                const cell =
                    row.cells[item.col];
                if (!cell)
                    return;
                cell.innerHTML =
                    item.value;
            });
        });
}

document.addEventListener("DOMContentLoaded", function () {
    loadTables();
});

function clearTable(tableName){

    localStorage.removeItem(
        "TABLE_" + tableName
    );

    location.reload();

}


/* edit LC No */




/* edit LC No */


/* edit row */


/* orgination chart */

document.querySelectorAll(".node").forEach(node=>{

    node.addEventListener("click",function(){

        alert(this.innerText);

    });

});

/* orgination chart */

/* ============================
   Railway Drawing Gallery
============================ */

const viewer = document.getElementById("viewer");
const viewerImg = document.getElementById("viewerImg");

let currentImage = "";

/* ============================
   Open Image
============================ */

function openImage(src) {

    currentImage = src;

    viewer.style.display = "flex";

    viewerImg.src = src;

}

/* ============================
   Close Image
============================ */

function closeImage() {

    viewer.style.display = "none";

}

/* ============================
   Close on ESC Key
============================ */

document.addEventListener("keydown", function (e) {

    if (e.key === "Escape") {

        closeImage();

    }

});

/* ============================
   Prevent Closing While Clicking Image
============================ */

viewerImg.addEventListener("click", function (e) {

    e.stopPropagation();

});

/* ============================
   Zoom Using Mouse Wheel
============================ */

let zoom = 1;

viewerImg.addEventListener("wheel", function (e) {

    e.preventDefault();

    if (e.deltaY < 0) {

        zoom += 0.1;

    } else {

        zoom -= 0.1;

    }

    if (zoom < 1)
        zoom = 1;

    if (zoom > 5)
        zoom = 5;

    viewerImg.style.transform = "scale(" + zoom + ")";

});

/* ============================
   Reset Zoom on Close
============================ */

viewer.addEventListener("click", function () {

    zoom = 1;

    viewerImg.style.transform = "scale(1)";

    closeImage();

});

/* ============================
   Lazy Loading Images
============================ */

window.addEventListener("load", function () {

    const images = document.querySelectorAll(".gallery img");

    images.forEach(function (img) {

        img.loading = "lazy";

    });

});

/* ============================
   Optional Search Function
============================ */


/* gallery */

function zoomImage(img){

    document.getElementById("imageViewer").style.display="flex";

    document.getElementById("zoomedImage").src=img.src;

}

function closeZoom(){

    document.getElementById("imageViewer").style.display="none";

}

/* gallery */

/* add update delete row */

/* Add New Row */

function addRow(link){

    const row = link.closest("tr");

    const tbody = row.parentNode;

    const clone = row.cloneNode(true);

    /* Remove any input boxes */

    clone.querySelectorAll("input,textarea").forEach(el=>{

        el.value="";

    });

    /* Clear text except first column */

    clone.querySelectorAll("td").forEach((td,index)=>{

        if(index==0){

            td.innerHTML=
            `
            0<br>

            <a href="javascript:void(0)"
               class="action-link edit-link"
               onclick="editRow(this)">Edit</a>

            |

            <a href="javascript:void(0)"
               class="action-link add-link"
               onclick="addRow(this)">Add</a>

            |

            <a href="javascript:void(0)"
               class="action-link delete-link"
               onclick="deleteRow(this)">Delete</a>
            `;

        }
        else{

            /* Skip LC hyperlink */

            if(td.querySelector(".lc-link")){

                td.innerHTML='';

            }else{

                td.innerHTML='';

            }

        }

    });

    tbody.insertBefore(clone,row.nextSibling);

    updateRowNumbers();

}

/* Delete Row */

function deleteRow(link){

    const row=link.closest("tr");

    const tbody=row.parentNode;

    if(tbody.rows.length==1){

        alert("Cannot delete last row.");

        return;

    }

    if(confirm("Delete this row?")){

        row.remove();

        updateRowNumbers();

    }

}

/* Update Serial Number */

function updateRowNumbers(){
    document.querySelectorAll("table tbody tr").forEach((row,index)=>{
        const first=row.cells[0];
        first.innerHTML=
        `
        ${index+1}<br>

        <a href="javascript:void(0)"
           class="action-link edit-link"
           onclick="editRow(this)">Edit</a>

        |

        <a href="javascript:void(0)"
           class="action-link add-link"
           onclick="addRow(this)">Add</a>

        |

        <a href="javascript:void(0)"
           class="action-link delete-link"
           onclick="deleteRow(this)">Delete</a>
        `;

    });

}

/* add update delete row */

/* zoom in zoom out image */


/* zoom in zoom out image */

document.querySelectorAll(".gallery-img").forEach(function(img){
    img.addEventListener("click",function(){
        // Replace with your PDF path if required
        let pdf=this.src
            .replace("img/gad/","pdf/")
            .replace(".png",".pdf");
       window.open(pdf,"_blank");
    });
});


/* form validation*/

/* pssa engage */

document.addEventListener("DOMContentLoaded", function () {

    fetch('/data/personnel.json')
        .then(response => response.json())
        .then(data => {

            let html = "";

            data.forEach(function (item) {

                html += `
                <div class="row table-row g-0">

                    <div class="col-12 d-md-none mobile-title">
                        Personnel Details
                    </div>

                    <div class="col-md-1 col-6 border-cell">
                        <span class="mobile-label">Sr No.</span>
                        ${item.srNo}
                    </div>

                    <div class="col-md-2 col-6 border-cell">
                        <span class="mobile-label">HQ</span>
                        ${item.hq}
                    </div>

                    <div class="col-md-3 col-12 border-cell">
                        <span class="mobile-label">Designation</span>
                        ${item.designation}
                    </div>

                    <div class="col-md-3 col-12 border-cell">
                        <span class="mobile-label">Nominated Work</span>
                        ${item.nominatedWork}
                    </div>

                    <div class="col-md-3 col-12 border-cell">
                        <span class="mobile-label">Name</span>
                        ${item.name}
                    </div>

                </div>`;
            });

            document.getElementById("personnelBody").innerHTML = html;
        });
});


/* pssa engage */





