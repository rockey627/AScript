var total = 1;

export function sum(a, b) {
    total += 1;
    return a + b;
}

export function fib(a) {
    if (a <= 1) return a;
    return fib(a - 1) + fib(a - 2);
}

export function getTotal() {
    return total;
}

/*
兼容nodeJS写法，import/require互相兼容
module.exports = { sum, fib }
*/
export default { sum, fib }