print("Введите строку")

stroka = input() #получаем строку

previstkobka = stroka.count("{")+stroka.count("}")
vtoroistroka = stroka.count("(") + stroka.count(")")
tristroka = stroka.count("[") + stroka.count("]")

if previstkobka == len(stroka):
    print("Строка существует")
elif vtoroistroka == len(stroka):
    print("Строка существует")
elif tristroka == len(stroka):
    print("Строка существует")
elif (tristroka + previstkobka + vtoroistroka == len(stroka)) and (previstkobka != 0) and (vtoroistroka != 0) and (tristroka != 0):
    print("Строка существует")
else:
    print("Строка не существует")