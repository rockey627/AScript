type Person struct {
    name string
    age int
}

func (p Person) GetAge() int {
    return p.age
}

func (p *Person) SetAge(age int) {
    p.age = age
}