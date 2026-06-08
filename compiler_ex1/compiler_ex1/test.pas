program test;

var
    a, b, c : integer;

begin
    a := 10;
    b := 20;

    c := a + b

    if c > 25 then
        c := c - 5;

    while a < b do
        a := a + 1;

    for a := 1 to 5 do
        c := c + a;
end