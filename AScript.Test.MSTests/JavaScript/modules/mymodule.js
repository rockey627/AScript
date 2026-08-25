
var total = 1;

export function sum(a, b) {
    total += 1;
    return a + b;
}

export function fib(a) {
    if (a <= 1) return 1;
    return a + fib(a - 1);
}

export function getTotal() {
    return total;
}

export default {
    sum,
    fib
}