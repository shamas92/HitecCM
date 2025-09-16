"use strict";

// Stripped version: removed all sidebar / menu toggle logic so a fresh sidebar can be implemented later without conflicts.
!function (NioApp, $) {
  "use strict";

  NioApp.Package.name = "DashLite";
  NioApp.Package.version = "2.3";
  var $win = $(window),
      $body = $('body'),
      $doc = $(document);

  function extend(obj, ext) { Object.keys(ext).forEach(function (k) { obj[k] = ext[k]; }); return obj; }

  // Basic utilities kept (validation, editors, etc.)
  NioApp.Prettify = function () { window.prettyPrint && prettyPrint(); };

  NioApp.Copied = function () {
    var clip = '.clipboard-init', target = '.clipboard-text';
    function feedback(el, state) {
      var $elm = $(el), $elp = $elm.parent(), copy = { text: 'Copy', done: 'Copied', fail: 'Failed' }, data = { text: $elm.data('clip-text'), done: $elm.data('clip-success'), fail: $elm.data('clip-error') };
      copy.text = data.text || copy.text; copy.done = data.done || copy.done; copy.fail = data.fail || copy.fail;
      var copytext = state === 'success' ? copy.done : copy.fail, addclass = state === 'success' ? 'clipboard-success' : 'clipboard-error';
      $elp.addClass(addclass).find(target).html(copytext);
      setTimeout(function () { $elp.removeClass('clipboard-success clipboard-error').find(target).html(copy.text).blur(); $elp.find('input').blur(); }, 2000);
    }
    if (window.ClipboardJS && ClipboardJS.isSupported()) {
      var clipboard = new ClipboardJS(clip);
      clipboard.on('success', function (e) { feedback(e.trigger, 'success'); e.clearSelection(); })
               .on('error', function (e) { feedback(e.trigger, 'error'); });
    } else { $(clip).css('display', 'none'); }
  };

  NioApp.Toast = function (msg, ttype, opt) {
    if (!window.toastr || !msg) return; var ticon = ttype === 'success' ? 'ni ni-check-circle-fill' : ttype === 'error' ? 'ni ni-cross-circle-fill' : ttype === 'warning' ? 'ni ni-alert-fill' : 'ni ni-info-fill';
    var def = { position: 'bottom-right', ui: '', icon: 'auto', clear: false }, attr = opt ? extend(def, opt) : def;
    attr.position = 'toast-' + (attr.position || 'bottom-right');
    var option = { closeButton: true, positionClass: attr.position + (attr.ui ? (' ' + attr.ui) : ''), preventDuplicates: true };
    toastr.options = extend(option, attr); toastr[ttype || 'info'](msg);
  };

  NioApp.Validate = function (elm, opt) {
    if (!$.fn.validate) return; if ($(elm).exists()) { $(elm).each(function () { var def = { errorElement: 'span' }, attr = opt ? extend(def, opt) : def; $(this).validate(attr); }); }
  };
  NioApp.Validate.init = function () { NioApp.Validate('.form-validate', { errorElement: 'span', errorClass: 'invalid', errorPlacement: function (err, el) { err.appendTo(el.parent()); } }); };

  NioApp.OtherInit = function () { /* placeholder for future sidebar-free init */ };

  // Minimal init queue
  NioApp.init = function () {
    NioApp.coms = NioApp.coms || { docReady: [], winLoad: [] };
    NioApp.coms.docReady.push(NioApp.OtherInit);
    NioApp.coms.docReady.push(NioApp.Copied);
    NioApp.coms.docReady.push(NioApp.Validate.init);
  };
  NioApp.init();

  // Execute queued docReady immediately (framework stripped)
  $(function(){ (NioApp.coms && NioApp.coms.docReady || []).forEach(function(fn){ try { fn(); } catch(e) { /* swallow */ } }); });

  return NioApp;
}(window.NioApp = window.NioApp || {}, jQuery);