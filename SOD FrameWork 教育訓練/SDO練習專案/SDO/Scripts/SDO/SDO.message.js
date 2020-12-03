function alertMessage(objMsg, callback, timeout) {
    top.showMessage(getMsg(objMsg), callback, timeout);
}
function confirmMessage(objMsg, callbackYes, callbackNo) {
    top.showConfirm(getMsg(objMsg), callbackYes, callbackNo);
}
function getMsg(objMsg) {
    if (objMsg) {
        switch (typeof objMsg) {
            case 'string':
            case 'number':
            case 'boolean':
                return objMsg;
            case 'object':
                return typeof objMsg.message === 'string' ? objMsg.message : typeof objMsg.statusText === 'string' ? objMsg.statusText : JSON.stringify(objMsg);
            default:
                return '';
        }
    }
}