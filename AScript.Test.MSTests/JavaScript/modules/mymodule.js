
function sum(a, b) {
    return a + b;
}

function fib(a) {
    if (a <= 1) return 1;
    return a + fib(a - 1);
}

export default {
    sum,
    fib
}