﻿/** 
CUSTOM TW CALENDAR
*/
!function (t, define) {
    define("kendo.calendar.min", ["kendo.core.min"], t)
}(function () {
    return function (t, e) {
        function n(t, e, n, a) { var r, i = t.getFullYear(), o = e.getFullYear(), s = n.getFullYear(); return i -= i % a, r = i + (a - 1), i < o && (i = o), r > s && (r = s), (i < 1911 ? 0 : i - 1911) + "-" + (r - 1911) }
        function a(t) { var e, n = 0, a = t.min, r = t.max, i = t.start, o = t.setter, l = t.build, u = t.weekNumberBuild, c = t.cells || 12, f = t.isWeekColumnVisible, d = t.perRow || 4, g = t.weekNumber || P, v = t.content || B, m = t.empty || E, h = t.html || '<table tabindex="0" role="grid" class="k-content k-meta-view" cellspacing="0"><tbody><tr role="row">'; for (f && (h += g(u(i))); n < c; n++) n > 0 && n % d === 0 && (h += '</tr><tr role="row">', f && (h += g(u(i)))), i = new Ft(i.getFullYear(), i.getMonth(), i.getDate(), 0, 0, 0), S(i, 0), e = l(i, n, t.disableDates), h += s(i, a, r) ? v(e) : m(e), o(i, 1); return h + "</tr></tbody></table>" }
        function r(t, e, n) { var a = t.getFullYear(), r = e.getFullYear(), i = r, o = 0; return n && (r -= r % n, i = r - r % n + n - 1), a > i ? o = 1 : a < r && (o = -1), o }
        function i() { var t = new Ft; return new Ft(t.getFullYear(), t.getMonth(), t.getDate()) }
        function o(t, e, n) { var a = i(); return t && (a = new Ft((+t))), e > a ? a = new Ft((+e)) : n < a && (a = new Ft((+n))), a }
        function s(t, e, n) { return +t >= +e && +t <= +n }
        function l(t, e) { return t.slice(e).concat(t.slice(0, e)) }
        function u(t, e, n) { e = e instanceof Ft ? e.getFullYear() : t.getFullYear() + n * e, t.setFullYear(e) }
        function c(e) { var n = t(this).hasClass("k-state-disabled"); n || t(this).toggleClass(nt, dt.indexOf(e.type) > -1 || e.type == ct) }
        function f(t) { t.preventDefault() }
        function d(t) { return A(t).calendars.standard }
        function g(t) { var n = yt[t.start], a = yt[t.depth], r = A(t.culture); t.format = O(t.format || r.calendars.standard.patterns.d), isNaN(n) && (n = 0, t.start = J), (a === e || a > n) && (t.depth = J), null === t.dates && (t.dates = []) }
        function v(t) { z && t.find("*").attr("unselectable", "on") }
        function m(t, e) { t.addClass("k-" + e) }
        function h(t, e) { for (var n = 0, a = e.length; n < a; n++) if (t === +e[n]) return !0; return !1 }
        function _(t, e) { return !!t && (t.getFullYear() === e.getFullYear() && t.getMonth() === e.getMonth() && t.getDate() === e.getDate()) }
        function p(t, e) { return !!t && (t.getFullYear() === e.getFullYear() && t.getMonth() === e.getMonth()) }
        function w(e) { return y.isFunction(e) ? e : t.isArray(e) ? D(e) : t.noop }
        function k(t) { var e, n = []; for (e = 0; e < t.length; e++) n.push(t[e].setHours(0, 0, 0, 0)); return n }
        function D(e) { var n, a, r, i, o, s = [], l = ["su", "mo", "tu", "we", "th", "fr", "sa"], u = "if (found) { return true } else {return false}"; if (e[0] instanceof Ft) s = k(e), n = "var found = date && $.inArray(date.setHours(0, 0, 0, 0),[" + s + "]) > -1;" + u; else { for (r = 0; r < e.length; r++) i = e[r].slice(0, 2).toLowerCase(), o = t.inArray(i, l), o > -1 && s.push(o); n = "var found = date && $.inArray(date.getDay(),[" + s + "]) > -1;" + u } return a = Function("date", n) }
        function b(t, e) { return t instanceof Date && e instanceof Date && (t = t.getTime(), e = e.getTime()), t === e } var F, y = window.kendo, Y = y.support, x = y.ui, C = x.Widget, M = y.keys, T = y.parseDate, S = y.date.adjustDST, N = y.date.weekInYear, O = y._extractFormat, V = y.template, A = y.getCulture, W = y.support.transitions, H = W ? W.css + "transform-origin" : "", B = V('<td#=data.cssClass# role="gridcell"><a tabindex="-1" class="k-link" href="\\#" data-#=data.ns#value="#=data.dateString#">#=data.value#</a></td>', { useWithBlock: !1 }), E = V('<td role="gridcell">&nbsp;</td>', { useWithBlock: !1 }), P = V('<td class="k-alt">#= data.weekNumber #</td>', { useWithBlock: !1 }), I = y.support.browser, z = I.msie && I.version < 9, R = y._outerHeight, U = y._outerWidth, j = ".kendoCalendar", G = "click" + j, q = "keydown" + j, L = "id", $ = "min", K = "left", Q = "slideIn", J = "month", X = "century", Z = "change", tt = "navigate", et = "value", nt = "k-state-hover", at = "k-state-disabled", rt = "k-state-focused", it = "k-other-month", ot = ' class="' + it + '"', st = "k-nav-today", lt = "td:has(.k-link)", ut = "blur" + j, ct = "focus", ft = ct + j, dt = Y.touch ? "touchstart" : "mouseenter", gt = Y.touch ? "touchstart" + j : "mouseenter" + j, vt = Y.touch ? "touchend" + j + " touchmove" + j : "mouseleave" + j, mt = 6e4, ht = 864e5, _t = "_prevArrow", pt = "_nextArrow", wt = "aria-disabled", kt = "aria-selected", Dt = t.proxy, bt = t.extend, Ft = Date, yt = { month: 0, year: 1, decade: 2, century: 3 }, Yt = C.extend({
            init: function (e, n) { var a, r, s = this; C.fn.init.call(s, e, n), e = s.wrapper = s.element, n = s.options, n.url = window.unescape(n.url), s.options.disableDates = w(s.options.disableDates), s._templates(), s._header(), s._footer(s.footer), r = e.addClass("k-widget k-calendar " + (n.weekNumber ? " k-week-number" : "")).on(gt + " " + vt, lt, c).on(q, "table.k-content", Dt(s._move, s)).on(G, lt, function (e) { var n = e.currentTarget.firstChild, a = s._toDateObject(n); n.href.indexOf("#") != -1 && e.preventDefault(), "month" == s._view.name && s.options.disableDates(a) || s._click(t(n)) }).on("mouseup" + j, "table.k-content, .k-footer", function () { s._focusView(s.options.focusOnNav !== !1) }).attr(L), r && (s._cellID = r + "_cell_selected"), g(n), a = T(n.value, n.format, n.culture), s._index = yt[n.start], s._current = new Ft((+o(a, n.min, n.max))), s._addClassProxy = function () { if (s._active = !0, s._cell.hasClass(at)) { var t = s._view.toDateString(i()); s._cell = s._cellByDate(t) } s._cell.addClass(rt) }, s._removeClassProxy = function () { s._active = !1, s._cell.removeClass(rt) }, s.value(a), y.notify(s) }, options: { name: "Calendar", value: null, min: new Ft(1900, 0, 1), max: new Ft(2099, 11, 31), dates: [], url: "", culture: "", footer: "", format: "", month: {}, weekNumber: !1, start: J, depth: J, animation: { horizontal: { effects: Q, reverse: !0, duration: 500, divisor: 2 }, vertical: { effects: "zoomIn", duration: 400 } } }, events: [Z, tt], setOptions: function (t) { var e = this; g(t), t.disableDates = w(t.disableDates), C.fn.setOptions.call(e, t), e._templates(), e._footer(e.footer), e._index = yt[e.options.start], e.navigate() }, destroy: function () { var t = this, e = t._today; t.element.off(j), t._title.off(j), t[_t].off(j), t[pt].off(j), y.destroy(t._table), e && y.destroy(e.off(j)), C.fn.destroy.call(t) }, current: function () { return this._current }, view: function () { return this._view }, focus: function (t) { t = t || this._table, this._bindTable(t), t.focus() }, min: function (t) { return this._option($, t) }, max: function (t) { return this._option("max", t) }, navigateToPast: function () { this._navigate(_t, -1) }, navigateToFuture: function () { this._navigate(pt, 1) }, navigateUp: function () { var t = this, e = t._index; t._title.hasClass(at) || t.navigate(t._current, ++e) }, navigateDown: function (t) { var n = this, a = n._index, r = n.options.depth; if (t) return a === yt[r] ? (b(n._value, n._current) && b(n._value, t) || (n.value(t), n.trigger(Z)), e) : (n.navigate(t, --a), e) }, navigate: function (n, a) { var r, i, s, l, u, c, f, d, g, h, _, p, w, k, D, b, y; a = isNaN(a) ? yt[a] : a, r = this, i = r.options, s = i.culture, l = i.min, u = i.max, c = r._title, f = r._table, d = r._oldTable, g = r._value, h = r._current, _ = n && +n > +h, p = a !== e && a !== r._index, n || (n = h), r._current = n = new Ft((+o(n, l, u))), a === e ? a = r._index : r._index = a, r._view = k = F.views[a], D = k.compare, b = a === yt[X], c.toggleClass(at, b).attr(wt, b), b = D(n, l) < 1, r[_t].toggleClass(at, b).attr(wt, b), b = D(n, u) > -1, r[pt].toggleClass(at, b).attr(wt, b), f && d && d.data("animating") && (d.kendoStop(!0, !0), f.kendoStop(!0, !0)), r._oldTable = f, f && !r._changeView || (c.html(k.title(n, l, u, s)), r._table = w = t(k.content(bt({ min: l, max: u, date: n, url: i.url, dates: i.dates, format: i.format, culture: s, disableDates: i.disableDates, isWeekColumnVisible: i.weekNumber }, r[k.name]))), m(w, k.name), v(w), y = f && f.data("start") === w.data("start"), r._animate({ from: f, to: w, vertical: p, future: _, replace: y }), r.trigger(tt), r._focus(n)), a === yt[i.depth] && g && !r.options.disableDates(g) && r._class("k-state-selected", g), r._class(rt, n), !f && r._cell && r._cell.removeClass(rt), r._changeView = !0 }, value: function (t) { var n = this, a = n._view, r = n.options, i = n._view, o = r.min, l = r.max; return t === e ? n._value : (null === t && (n._current = new Date(n._current.getFullYear(), n._current.getMonth(), n._current.getDate())), t = T(t, r.format, r.culture), null !== t && (t = new Ft((+t)), s(t, o, l) || (t = null)), null !== t && n.options.disableDates(t) ? n._value === e && (n._value = null) : n._value = t, i && null === t && n._cell ? n._cell.removeClass("k-state-selected") : (n._changeView = !t || a && 0 !== a.compare(t, n._current), n.navigate(t)), e) }, _move: function (e) { var n, a, r, i, l = this, u = l.options, c = e.keyCode, f = l._view, d = l._index, g = l.options.min, v = l.options.max, m = new Ft((+l._current)), h = y.support.isRtl(l.wrapper), _ = l.options.disableDates; return e.target === l._table[0] && (l._active = !0), e.ctrlKey ? c == M.RIGHT && !h || c == M.LEFT && h ? (l.navigateToFuture(), a = !0) : c == M.LEFT && !h || c == M.RIGHT && h ? (l.navigateToPast(), a = !0) : c == M.UP ? (l.navigateUp(), a = !0) : c == M.DOWN && (l._click(t(l._cell[0].firstChild)), a = !0) : (c == M.RIGHT && !h || c == M.LEFT && h ? (n = 1, a = !0) : c == M.LEFT && !h || c == M.RIGHT && h ? (n = -1, a = !0) : c == M.UP ? (n = 0 === d ? -7 : -4, a = !0) : c == M.DOWN ? (n = 0 === d ? 7 : 4, a = !0) : c == M.ENTER ? (l._click(t(l._cell[0].firstChild)), a = !0) : c == M.HOME || c == M.END ? (r = c == M.HOME ? "first" : "last", i = f[r](m), m = new Ft(i.getFullYear(), i.getMonth(), i.getDate(), m.getHours(), m.getMinutes(), m.getSeconds(), m.getMilliseconds()), a = !0) : c == M.PAGEUP ? (a = !0, l.navigateToPast()) : c == M.PAGEDOWN && (a = !0, l.navigateToFuture()), (n || r) && (r || f.setDate(m, n), _(m) && (m = l._nextNavigatable(m, n)), s(m, g, v) && l._focus(o(m, u.min, u.max)))), a && e.preventDefault(), l._current }, _nextNavigatable: function (t, e) { var n = this, a = !0, r = n._view, i = n.options.min, o = n.options.max, l = n.options.disableDates, u = new Date(t.getTime()); for (r.setDate(u, -e); a;) { if (r.setDate(t, e), !s(t, i, o)) { t = u; break } a = l(t) } return t }, _animate: function (t) { var e = this, n = t.from, a = t.to, r = e._active; n ? n.parent().data("animating") ? (n.off(j), n.parent().kendoStop(!0, !0).remove(), n.remove(), a.insertAfter(e.element[0].firstChild), e._focusView(r)) : !n.is(":visible") || e.options.animation === !1 || t.replace ? (a.insertAfter(n), n.off(j).remove(), e._focusView(r)) : e[t.vertical ? "_vertical" : "_horizontal"](n, a, t.future) : (a.insertAfter(e.element[0].firstChild), e._bindTable(a)) }, _horizontal: function (t, e, n) { var a = this, r = a._active, i = a.options.animation.horizontal, o = i.effects, s = U(t); o && o.indexOf(Q) != -1 && (t.add(e).css({ width: s }), t.wrap("<div/>"), a._focusView(r, t), t.parent().css({ position: "relative", width: 2 * s, "float": K, "margin-left": n ? 0 : -s }), e[n ? "insertAfter" : "insertBefore"](t), bt(i, { effects: Q + ":" + (n ? "right" : K), complete: function () { t.off(j).remove(), a._oldTable = null, e.unwrap(), a._focusView(r) } }), t.parent().kendoStop(!0, !0).kendoAnimate(i)) }, _vertical: function (t, e) { var n, a, r = this, i = r.options.animation.vertical, o = i.effects, s = r._active; o && o.indexOf("zoom") != -1 && (e.css({ position: "absolute", top: R(t.prev()), left: 0 }).insertBefore(t), H && (n = r._cellByDate(r._view.toDateString(r._current)), a = n.position(), a = a.left + parseInt(n.width() / 2, 10) + "px " + (a.top + parseInt(n.height() / 2, 10) + "px"), e.css(H, a)), t.kendoStop(!0, !0).kendoAnimate({ effects: "fadeOut", duration: 600, complete: function () { t.off(j).remove(), r._oldTable = null, e.css({ position: "static", top: 0, left: 0 }), r._focusView(s) } }), e.kendoStop(!0, !0).kendoAnimate(i)) }, _cellByDate: function (e) { return this._table.find("td:not(." + it + ")").filter(function () { return t(this.firstChild).attr(y.attr(et)) === e }) }, _class: function (e, n) { var a, r = this, i = r._cellID, o = r._cell, s = r._view.toDateString(n); o && o.removeAttr(kt).removeAttr("aria-label").removeAttr(L), n && "month" == r._view.name && (a = r.options.disableDates(n)), o = r._table.find("td:not(." + it + ")").removeClass(e).filter(function () { return t(this.firstChild).attr(y.attr(et)) === s }).attr(kt, !0), (e === rt && !r._active && r.options.focusOnNav !== !1 || a) && (e = ""), o.addClass(e), o[0] && (r._cell = o), i && (o.attr(L, i), r._table.removeAttr("aria-activedescendant").attr("aria-activedescendant", i)) }, _bindTable: function (t) { t.on(ft, this._addClassProxy).on(ut, this._removeClassProxy) }, _click: function (t) { var e = this, n = e.options, a = new Date((+e._current)), r = e._toDateObject(t); S(r, 0), "month" == e._view.name && e.options.disableDates(r) && (r = e._value), e._view.setDate(a, r), e.navigateDown(o(a, n.min, n.max)) }, _focus: function (t) { var e = this, n = e._view; 0 !== n.compare(t, e._current) ? e.navigate(t) : (e._current = t, e._class(rt, t)) }, _focusView: function (t, e) { t && this.focus(e) }, _footer: function (n) {
                var a = this, r = i(), o = a.element, s = o.find(".k-footer"); return n ? (s[0] || (s = t('<div class="k-footer"><a href="#" class="k-link k-nav-today"></a></div>').appendTo(o)),
                    a._today = s.show().find(".k-link").html(n(r)).attr("title", y.toString(r, "D", a.options.culture)), a._toggle(), e) : (a._toggle(!1), s.hide(), e)
            }, _header: function () { var t, e = this, n = e.element; n.find(".k-header")[0] || n.html('<div class="k-header"><a href="#" role="button" class="k-link k-nav-prev"><span class="k-icon k-i-arrow-60-left"></span></a><a href="#" role="button" aria-live="assertive" aria-atomic="true" class="k-link k-nav-fast"></a><a href="#" role="button" class="k-link k-nav-next"><span class="k-icon k-i-arrow-60-right"></span></a></div>'), t = n.find(".k-link").on(gt + " " + vt + " " + ft + " " + ut, c).click(!1), e._title = t.eq(1).on(G, function () { e._active = e.options.focusOnNav !== !1, e.navigateUp() }), e[_t] = t.eq(0).on(G, function () { e._active = e.options.focusOnNav !== !1, e.navigateToPast() }), e[pt] = t.eq(2).on(G, function () { e._active = e.options.focusOnNav !== !1, e.navigateToFuture() }) }, _navigate: function (t, e) { var n = this, a = n._index + 1, r = new Ft((+n._current)); t = n[t], t.hasClass(at) || (a > 3 ? r.setFullYear(r.getFullYear() + 100 * e) : F.views[a].setDate(r, e), n.navigate(r)) }, _option: function (t, n) { var a, r = this, i = r.options, o = r._value || r._current; return n === e ? i[t] : (n = T(n, i.format, i.culture), n && (i[t] = new Ft((+n)), a = t === $ ? n > o : o > n, (a || p(o, n)) && (a && (r._value = null), r._changeView = !0), r._changeView || (r._changeView = !(!i.month.content && !i.month.empty)), r.navigate(r._value), r._toggle()), e) }, _toggle: function (t) { var n = this, a = n.options, r = n.options.disableDates(i()), o = n._today; t === e && (t = s(i(), a.min, a.max)), o && (o.off(G), t && !r ? o.addClass(st).removeClass(at).on(G, Dt(n._todayClick, n)) : o.removeClass(st).addClass(at).on(G, f)) }, _todayClick: function (t) { var e = this, n = yt[e.options.depth], a = e.options.disableDates, r = i(); t.preventDefault(), a(r) || (0 === e._view.compare(e._current, r) && e._index == n && (e._changeView = !1), e._value = r, e.navigate(r, n), e.trigger(Z)) }, _toDateObject: function (e) { var n = t(e).attr(y.attr(et)).split("/"); return n = new Ft(n[0], n[1], n[2]) },
            _templates: function () {
                var t = this, e = t.options, n = e.footer, a = e.month, r = a.content, i = a.weekNumber, o = a.empty; t.month = { content: V('<td#=data.cssClass# role="gridcell"><a tabindex="-1" class="k-link#=data.linkClass#" href="#=data.url#" ' + y.attr("value") + '="#=data.dateString#" title="#=data.title#">' + (r || "#=data.value#") + "</a></td>", { useWithBlock: !!r }), empty: V('<td role="gridcell">' + (o || "&nbsp;") + "</td>", { useWithBlock: !!o }), weekNumber: V('<td class="k-alt">' + (i || "#= data.weekNumber #") + "</td>", { useWithBlock: !!i }) },
                    t.footer = n !== !1 ? V(n || '#= "民國" + (kendo.toString(data,"yyyy","' + e.culture + '") - 1911) + "年" + kendo.toString(data,"M","' + e.culture + '") #', { useWithBlock: !1 }) : null
            }
        }); x.plugin(Yt), F = {
            firstDayOfMonth: function (t) { return new Ft(t.getFullYear(), t.getMonth(), 1) }, firstVisibleDay: function (t, e) { e = e || y.culture().calendar; for (var n = e.firstDay, a = new Ft(t.getFullYear(), t.getMonth(), 0, t.getHours(), t.getMinutes(), t.getSeconds(), t.getMilliseconds()); a.getDay() != n;) F.setTime(a, -1 * ht); return a }, setTime: function (t, e) { var n = t.getTimezoneOffset(), a = new Ft(t.getTime() + e), r = a.getTimezoneOffset() - n; t.setTime(a.getTime() + r * mt) },
            views: [{
                name: J, title: function (t, e, n, a) { return "民國" + (t.getFullYear() - 1911) + "年 " + d(a).months.names[t.getMonth()] },
                content: function (t) { var e = this, n = 0, r = t.min, i = t.max, o = t.date, s = t.dates, u = t.format, c = t.culture, f = t.url, g = t.isWeekColumnVisible, v = f && s[0], m = d(c), _ = m.firstDay, p = m.days, w = l(p.names, _), k = l(p.namesShort, _), D = F.firstVisibleDay(o, m), b = e.first(o), Y = e.last(o), x = e.toDateString, C = new Ft, M = '<table tabindex="0" role="grid" class="k-content" cellspacing="0" data-start="' + x(D) + '"><thead><tr role="row">'; for (g && (M += '<th scope="col" class="k-alt"></th>'); n < 7; n++) M += '<th scope="col" title="' + w[n] + '">' + k[n] + "</th>"; return C = new Ft(C.getFullYear(), C.getMonth(), C.getDate()), S(C, 0), C = +C, a({ cells: 42, perRow: 7, html: M += '</tr></thead><tbody><tr role="row">', start: D, isWeekColumnVisible: g, weekNumber: t.weekNumber, min: new Ft(r.getFullYear(), r.getMonth(), r.getDate()), max: new Ft(i.getFullYear(), i.getMonth(), i.getDate()), content: t.content, empty: t.empty, setter: e.setDate, disableDates: t.disableDates, build: function (t, e, n) { var a = [], r = t.getDay(), i = "", o = "#"; return (t < b || t > Y) && a.push(it), n(t) && a.push(at), +t === C && a.push("k-today"), 0 !== r && 6 !== r || a.push("k-weekend"), v && h(+t, s) && (o = f.replace("{0}", y.toString(t, u, c)), i = " k-action-link"), { date: t, dates: s, ns: y.ns, title: y.toString(t, "D", c), value: t.getDate(), dateString: x(t), cssClass: a[0] ? ' class="' + a.join(" ") + '"' : "", linkClass: i, url: o } }, weekNumberBuild: function (t) { return { weekNumber: N(t, t), currentDate: t } } }) }, first: function (t) { return F.firstDayOfMonth(t) }, last: function (t) { var e = new Ft(t.getFullYear(), t.getMonth() + 1, 0), n = F.firstDayOfMonth(t), a = Math.abs(e.getTimezoneOffset() - n.getTimezoneOffset()); return a && e.setHours(n.getHours() + a / 60), e }, compare: function (t, e) { var n, a = t.getMonth(), r = t.getFullYear(), i = e.getMonth(), o = e.getFullYear(); return n = r > o ? 1 : r < o ? -1 : a == i ? 0 : a > i ? 1 : -1 }, setDate: function (t, e) { var n = t.getHours(); e instanceof Ft ? t.setFullYear(e.getFullYear(), e.getMonth(), e.getDate()) : F.setTime(t, e * ht), S(t, n) }, toDateString: function (t) { return t.getFullYear() + "/" + t.getMonth() + "/" + t.getDate() }
            }, {
                name: "year", title: function (t) { return "民國" + (t.getFullYear() - 1911) + "年" },
                content: function (t) { var e = d(t.culture).months.namesAbbr, n = this.toDateString, r = t.min, i = t.max; return a({ min: new Ft(r.getFullYear(), r.getMonth(), 1), max: new Ft(i.getFullYear(), i.getMonth(), 1), start: new Ft(t.date.getFullYear(), 0, 1), setter: this.setDate, build: function (t) { return { value: e[t.getMonth()], ns: y.ns, dateString: n(t), cssClass: "" } } }) }, first: function (t) { return new Ft(t.getFullYear(), 0, t.getDate()) }, last: function (t) { return new Ft(t.getFullYear(), 11, t.getDate()) }, compare: function (t, e) { return r(t, e) }, setDate: function (t, e) { var n, a = t.getHours(); e instanceof Ft ? (n = e.getMonth(), t.setFullYear(e.getFullYear(), n, t.getDate()), n !== t.getMonth() && t.setDate(0)) : (n = t.getMonth() + e, t.setMonth(n), n > 11 && (n -= 12), n > 0 && t.getMonth() != n && t.setDate(0)), S(t, a) }, toDateString: function (t) { return t.getFullYear() + "/" + t.getMonth() + "/1" }
            },
            {
                ame: "decade", title: function (t, e, a) { return "民國 " + n(t, e, a, 10) + " 年" },
                content: function (t) {
                    var e = t.date.getFullYear(), n = this.toDateString; return a({
                        start: new Ft(e - e % 10 - 1, 0, 1), min: new Ft(1912, 0, 1), max: new Ft(t.max.getFullYear(), 0, 1), setter: this.setDate,
                        build: function (t, e) { return { value: t.getFullYear() - 1911, ns: y.ns, dateString: n(t), cssClass: 0 === e || 11 == e ? ot : "" } }
                    })
                }, first: function (t) { var e = t.getFullYear(); return new Ft(e - e % 10, t.getMonth(), t.getDate()) }, last: function (t) { var e = t.getFullYear(); return new Ft(e - e % 10 + 9, t.getMonth(), t.getDate()) }, compare: function (t, e) { return r(t, e, 10) }, setDate: function (t, e) { u(t, e, 1) }, toDateString: function (t) { return t.getFullYear() + "/0/1" }
            },
            {
                name: X, title: function (t, e, a) { return "民國 " + n(t, e, a, 100) + " 年" },
                content: function (t) {
                    var e = t.date.getFullYear(), n = 1912, r = t.max.getFullYear(), i = this.toDateString, o = n, s = r; return o -= o % 10, s -= s % 10, s - o < 10 && (s = o + 9), a({
                        start: new Ft(e - e % 100 - 10, 0, 1), min: new Ft(o, 0, 1), max: new Ft(s, 0, 1), setter: this.setDate,
                        build: function (t, e) { var a = t.getFullYear(), o = a + 9; return a < n && (a = n), o > r && (o = r), { ns: y.ns, value: (a - 1911) + " - " + (o - 1911), dateString: i(t), cssClass: 0 === e || 11 == e ? ot : "" } }
                    })
                }, first: function (t) { var e = t.getFullYear(); return new Ft(e - e % 100, t.getMonth(), t.getDate()) }, last: function (t) { var e = t.getFullYear(); return new Ft(e - e % 100 + 99, t.getMonth(), t.getDate()) }, compare: function (t, e) { return r(t, e, 100) }, setDate: function (t, e) { u(t, e, 10) }, toDateString: function (t) { var e = t.getFullYear(); return e - e % 10 + "/0/1" }
            }]
        }, F.isEqualDatePart = _, F.isEqualDate = b, F.makeUnselectable = v, F.restrictValue = o, F.isInRange = s, F.addClassToViewContainer = m, F.normalize = g, F.viewsEnum = yt, F.disabled = w, y.calendar = F
    }(window.kendo.jQuery), window.kendo
}, "function" == typeof define && define.amd ? define : function (t, e, n) { (n || e)() });
//set datetimepicker 
function setTWDatetime(id, displayTime, cascadeFrom) {
    var setDate = kendo.parseDate($('#' + id).val().trim()), position = $('#' + id).offset();
    if (displayTime) {
        $('#_' + id).val(setDate == null ? '' : parseTWDatetime(setDate));
        $('#_' + id).kendoDateTimePicker({
            open: function (e) {
                if (cascadeFrom.length > 0)
                    $('#_' + id).data('kendoDateTimePicker').min(kendo.parseDate($('#' + cascadeFrom).val()))
                e.sender.value(kendo.parseDate($('#' + id).val()));
                $('#_' + id).hide();
            },
            close: function (e) {
                $('#' + id).val(e.sender.value() == null ? '' : kendo.toString(e.sender.value(), 'yyyy/MM/dd HH:mm'));
                $('#_' + id).val(e.sender.value() == null ? '' : parseTWDatetime(e.sender.value()));
                $('#_' + id).show();
            },
            change: function (e) {
                if (e.sender.value() == null)
                    e.sender.value(kendo.parseDate($('#' + id).val()));
                $('#' + id).val(e.sender.value() == null ? '' : kendo.toString(e.sender.value(), 'yyyy/MM/dd HH:mm'));
                $('#_' + id).val(e.sender.value() == null ? '' : parseTWDatetime(e.sender.value()));
                $('#_' + id).show();
            }
        });
    }
    else {
        $('#_' + id).val(setDate == null ? '' : parseTWDate(setDate));
        $('#_' + id).kendoDatePicker({
            open: function (e) {
                if (cascadeFrom.length > 0)
                    $('#_' + id).data('kendoDatePicker').min(kendo.parseDate($('#' + cascadeFrom).val()))
                e.sender.value(kendo.parseDate($('#' + id).val()));
                $('#_' + id).hide();
            },
            close: function (e) {
                $('#' + id).val(e.sender.value() == null ? '' : kendo.toString(e.sender.value(), 'yyyy/MM/dd'));
                $('#_' + id).val(e.sender.value() == null ? '' : parseTWDate(e.sender.value()));
                $('#_' + id).show();
            },
            change: function (e) {
                if (e.sender.value() == null)
                    e.sender.value(kendo.parseDate($('#' + id).val()));
                $('#' + id).val(e.sender.value() == null ? '' : kendo.toString(e.sender.value(), 'yyyy/MM/dd'));
                $('#_' + id).val(e.sender.value() == null ? '' : parseTWDate(e.sender.value()));
                $('#_' + id).show();
            }
        });
    }
    $('#' + id).hide().closest('form').on('reset', function (e) {
        setTimeout(function () {
            var setDate = kendo.parseDate($('#' + id).val().trim());
            if (displayTime)
                $('#_' + id).val(setDate == null ? '' : parseTWDatetime(setDate));
            else
                $('#_' + id).val(setDate == null ? '' : parseTWDate(setDate));
        }, 10)
    });
}
//parse date to TW datetime
function parseTWDatetime(d) {
    var twd = new Date(d);
    return kendo.toString(twd, 'yyyy') - 1911 + kendo.toString(twd, '/MM/dd HH:mm:ss');
};
//parse date to TW date
function parseTWDate(d) {
    var twd = new Date(d);
    return kendo.toString(twd, 'yyyy') - 1911 + kendo.toString(twd, '/MM/dd');
};
//change datetime
function changeDt(obj) {
    var chgDate = new Date($(obj).val().trim());
    $('#' + $(obj).attr('id').substring(1)).val($(obj).val().trim().length > 0 ? chgDate.getFullYear() > 1911 ? $(obj).val().trim() : (chgDate.getFullYear() + 1911) + kendo.toString(chgDate, '/MM/dd HH:mm') : '');
}
