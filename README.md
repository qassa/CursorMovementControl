# CursorMovementControl
This simple app allows to achieve realistic mouse movement (and robust movement)
#  Функционал
Данное приложение раелизует простую концепцию по автоматизированному управлению курсором. Приложение позволяет примерно имитировать человеческие движения курсором при серфинге или обычном управлении ПК.
Доступна возможность по строго перемещению по заданной траектории. Для запуска достаточно ввести 4 значения координат: точку начала и точку пермещения.
# Как реализовано
Для достижения эффекта "дрожащего" курсора при каждом новом перемещении весь сектор из 360° разбивается на 40 частей.

<img src="https://github.com/qassa/CursorMovementControl/blob/master/cursor_sector_concept.png" width="500" title="Cursor sector concept"/>
По одно из осей на каждом шаге вычисляется случайная величина (координата), значение на другой оси изменяется на фиксированную величину либо в положительную, либо в отрицательную сторону, в зависимости от четверти круга.

<img src="https://github.com/qassa/CursorMovementControl/blob/master/sector_explanation_smooth_method.png" width="800" title="Smooth movments explanation"/>
Для достижения эффекта точного перемещения использован такой же подход. Вместо случаной величины значение координаты вычисляется по обычному уравнению прямой.

<img src="https://github.com/qassa/CursorMovementControl/blob/master/sector_explanation_robust_method.png" width="800" title="Robust movments explanation"/>

# Как выглядит
<img src="https://github.com/qassa/CursorMovementControl/blob/master/form_cursor.jpg" title="Form presentation"/>

# Пример работы приложения
<a href="https://youtube.com/watch?v=g-_EEO11S8s"><img src="https://github.com/qassa/CursorMovementControl/blob/master/preview.jpg" width="600" title="Watch cursor's life"/></a>