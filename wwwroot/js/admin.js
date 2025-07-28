
// Dropdown //
function initCustomDropdowns() {
  const dropdowns = document.querySelectorAll('.custom-dropdown');
  console.log("Dropdowns encontrados:", dropdowns.length);

  dropdowns.forEach((dropdown, i) => {
    const selected = dropdown.querySelector('.dropdown-selected');
    const menu = dropdown.querySelector('.dropdown-menu');
    const links = menu?.querySelectorAll('a');
    const storageKey = dropdown.dataset.key || `dropdownIndex${i}`;

    if (!selected || !menu || !links || links.length === 0) return;

    let index = parseInt(localStorage.getItem(storageKey), 10);
    if (isNaN(index) || index < 0 || index >= links.length) {
      index = 0;
    }
    localStorage.setItem(storageKey, index);

    links.forEach(link => link.classList.remove('active'));
    links[index].classList.add('active');

    selected.addEventListener('click', () => {
      dropdown.classList.toggle('open');
    });

    document.addEventListener('click', (e) => {
      if (!dropdown.contains(e.target)) {
        dropdown.classList.remove('open');
      }
    });

    links.forEach((link, idx) => {
      link.addEventListener('click', e => {
        e.preventDefault();

        const active = dropdown.querySelector('a.active');
        if (active !== link) {
          active?.classList.remove('active');
          link.classList.add('active');
          localStorage.setItem(storageKey, idx);
        }

        dropdown.classList.remove('open');

        const url = link.dataset.url || link.href;
        if (url) window.location.href = url;
      });
    });
  });
}

document.addEventListener('DOMContentLoaded', initCustomDropdowns);

// Show-Hide Modals //
let adminAEliminarId = null; 

    document.addEventListener("DOMContentLoaded", function(){
        window.mostrarModal = function(id){
            adminAEliminarId = id;
            document.getElementById("modalDelete").style.display = "flex";
        }
        window.mostrarModal2 = function(){
        document.getElementById("idAdministradorEliminarConfirm").value = adminAEliminarId;
        document.getElementById("modalConfirmarEliminacion").style.display = "flex";
        }
    })
    function ocultarModal() {
        document.getElementById("modalDelete").style.display = "none";
    }
    function ocultarModal2(){
        document.getElementById("modalConfirmarEliminacion").style.display = "none";
    }
    function previewFoto(event) {
        const reader = new FileReader();
        reader.onload = function () {
            document.getElementById('previewImage').src = reader.result;
        }
        reader.readAsDataURL(event.target.files[0]);
    }