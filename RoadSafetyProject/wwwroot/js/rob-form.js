/* =========================================================
   ROB Project Sheet — validation & interaction logic
   ========================================================= */
(function () {
  'use strict';

  const form = document.getElementById('robForm');
  const statusEl = document.getElementById('formStatus');
  const progressFill = document.getElementById('progressFill');
  const toast = document.getElementById('robToast');
  const resetBtn = document.getElementById('resetBtn');

  // All fields we track for the progress indicator (required + optional)
  const allFields = Array.from(form.querySelectorAll('input, select, textarea'));
  const requiredFields = Array.from(form.querySelectorAll('[required]'));

  /* ---------- Progress bar: % of ALL fields with a value ---------- */
  function updateProgress() {
    const filled = allFields.filter(f => (f.value || '').trim() !== '').length;
    const pct = Math.round((filled / allFields.length) * 100);
    progressFill.style.width = pct + '%';
  }

  /* ---------- Per-field validity message sync ---------- */
  function markField(field) {
    const feedback = field.closest('.mb-3, .col-md-2, .col-md-3, .col-md-4, .col-md-6')
      ?.querySelector('.invalid-feedback');
    const valid = field.checkValidity();

    field.classList.remove('is-valid', 'is-invalid');
    if (field.value.trim() === '' && !form.classList.contains('was-validated')) {
      // field not yet touched by a submit attempt — no state until blurred with content expected
      if (document.activeElement === field) return;
    }
    field.classList.add(valid ? 'is-valid' : 'is-invalid');
    if (!valid && feedback) {
      feedback.textContent = field.validationMessage || feedback.textContent;
    }
  }

  allFields.forEach(field => {
    field.addEventListener('input', () => {
      updateProgress();
      if (form.classList.contains('was-validated')) markField(field);
    });
    field.addEventListener('blur', () => markField(field));
  });

  /* ---------- Extra semantic validation beyond HTML5 ---------- */
  function customValidate() {
    let ok = true;

    // Year of sanction: must not be in the future beyond current year
    const year = document.getElementById('yearSanction');
    const currentYear = new Date().getFullYear();
    if (year.value && Number(year.value) > currentYear) {
      year.setCustomValidity(`Year cannot be later than ${currentYear}.`);
      ok = false;
    } else {
      year.setCustomValidity('');
    }

    // Skew angle range guard (in case browser number step bypasses min/max on paste)
    const skew = document.getElementById('skewAngle');
    if (skew.value && (Number(skew.value) < 0 || Number(skew.value) > 90)) {
      skew.setCustomValidity('Skew angle must be between 0° and 90°.');
      ok = false;
    } else {
      skew.setCustomValidity('');
    }

    return ok;
  }

  /* ---------- Submit handling ---------- */
  form.addEventListener('submit', function (e) {
    e.preventDefault();
    e.stopPropagation();

    customValidate();
    form.classList.add('was-validated');
    allFields.forEach(markField);

    if (!form.checkValidity()) {
      const firstInvalid = form.querySelector(':invalid');
      if (firstInvalid) {
        firstInvalid.scrollIntoView({ behavior: 'smooth', block: 'center' });
        firstInvalid.focus({ preventScroll: true });
      }
      statusEl.textContent = 'Please correct the highlighted fields before saving.';
      statusEl.classList.remove('ok');
      statusEl.classList.add('err');
      return;
    }

    statusEl.textContent = '';
    statusEl.classList.remove('err');

    // Collect all form data into a plain object, then JSON-stringify it.
    const formData = new FormData(form);
    const jsonData = {};
    formData.forEach((value, key) => {
      jsonData[key] = value;
    });

    const jsonString = JSON.stringify(jsonData, null, 2);

    console.log('ROB Form Data (JSON):');
    console.log(jsonString);
    // console.table(jsonData); // uncomment for a tabular view instead

    showToast('Details saved successfully.');
    statusEl.textContent = 'Saved ' + new Date().toLocaleString();
    statusEl.classList.add('ok');
  });

  /* ---------- Reset handling ---------- */
  resetBtn.addEventListener('click', function () {
    form.reset();
    form.classList.remove('was-validated');
    allFields.forEach(f => f.classList.remove('is-valid', 'is-invalid'));
    statusEl.textContent = '';
    statusEl.classList.remove('ok', 'err');
    updateProgress();
  });

  /* ---------- Toast ---------- */
  let toastTimer;
  function showToast(message) {
    toast.textContent = message;
    toast.classList.add('show');
    clearTimeout(toastTimer);
    toastTimer = setTimeout(() => toast.classList.remove('show'), 3200);
  }

  // Initial paint
  updateProgress();
})();
