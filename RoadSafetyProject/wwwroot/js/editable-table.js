// editable-table.js
// Starter generic editable table engine
const EditableTable=(function(){
function key(id){return 'editable_'+id;}
function editRow(link){
 const row=link.closest('tr');
 const table=row.closest('table');
 if(link.textContent.trim()==='Edit'){
   link.textContent='Save';
   [...row.cells].forEach((c,i)=>{
     if(i===0) return;
     if(c.querySelector('a')) return;
     if(c.hasAttribute('rowspan')||c.hasAttribute('colspan')) return;
     const v=c.innerHTML.replace(/<br\s*\/?/gi,'\n');
     c.innerHTML='<textarea class="et-input">'+v+'</textarea>';
   });
 }else{
   link.textContent='Edit';
   row.querySelectorAll('textarea').forEach(t=>{
      t.parentElement.innerHTML=t.value.replace(/\n/g,'<br>');
   });
   localStorage.setItem(key(table.id),table.tBodies[0].innerHTML);
 }
}
function init(id){
 const t=document.getElementById(id);
 if(!t) return;
 const html=localStorage.getItem(key(id));
 if(html){
   t.tBodies[0].innerHTML=html;
   t.querySelectorAll('.edit-link').forEach(a=>a.onclick=function(){EditableTable.editRow(this);});
 }
}
function reset(id){localStorage.removeItem(key(id));location.reload();}
return {init,editRow,reset};
})();