document.addEventListener('DOMContentLoaded', () => {
  const admin = document.querySelector('.admin');
  const selected = admin.querySelector('.admin-selected-usu');
  const dropdown = admin.querySelector('.admin-dropdown');
  const links = dropdown.querySelectorAll('a');

  let index = parseInt(localStorage.getItem('adminIndex'), 10) || 0;

    // Validar índice
    if (index < 0 || index >= links.length) {
      index = 0;
      localStorage.setItem('adminIndex', index);
    }

  // Inicializar estado visual
  links.forEach(l => l.classList.remove('active'));
  if (links[index]) links[index].classList.add('active');

  // Abrir/cerrar el menú al hacer clic
  selected.addEventListener('click', () => {
    admin.classList.toggle('open');
  });

  // Cerrar si se hace clic fuera
  document.addEventListener('click', (e) => {
    if (!admin.contains(e.target)) {
      admin.classList.remove('open');
    }
  });

  // Click handler
  links.forEach((el, i) => el.addEventListener('click', e => {
    admin.querySelector('a.active')?.classList.remove('active');

    el.classList.add('active');
    localStorage.setItem('adminIndex', i);
    admin.classList.remove('open'); // Cierra menú

    //  window.location.href = el.getAttribute('@Url.Action("Empleados", "Administrador")');
  }));
  });