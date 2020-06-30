from __future__ import annotations
from abc import ABC, abstractmethod
from typing import List


#https://refactoring.guru/ru/design-patterns/strategy/python/example


class Context():
    """
    Контекст определяет интерфейс, представляющий интерес для клиентов.
    """

    def __init__(self, strategy: SortedStrategyBase) -> None:
        """
        Обычно Контекст принимает стратегию через конструктор, а также
        предоставляет сеттер для её изменения во время выполнения.
        """

        self._strategy = strategy

    @property
    def strategy(self) -> SortedStrategyBase:
        """
        Контекст хранит ссылку на один из объектов Стратегии. Контекст не знает
        конкретного класса стратегии. Он должен работать со всеми стратегиями
        через интерфейс Стратегии.
        """

        return self._strategy

    @strategy.setter
    def strategy(self, strategy: SortedStrategyBase) -> None:
        """
        Обычно Контекст позволяет заменить объект Стратегии во время выполнения.
        """

        self._strategy = strategy

    def do_some_business_logic(self) -> None:
        """
        Вместо того, чтобы самостоятельно реализовывать множественные версии
        алгоритма, Контекст делегирует некоторую работу объекту Стратегии.
        """

        # ...

        print("Context: Sorting data using the strategy (not sure how it'll do it)")
        print(self._strategy.description)
        result = self._strategy.do_algorithm(["a", "b", "c", "d", "e"])
        print(",".join(result))

        # ...


class SortedStrategyBase(ABC):
    """
    Интерфейс Стратегии объявляет операции, общие для всех поддерживаемых версий
    некоторого алгоритма.

    Контекст использует этот интерфейс для вызова алгоритма, определённого
    Конкретными Стратегиями.
    """

    @abstractmethod
    def do_algorithm(self, data: List):
        pass

    @property
    @abstractmethod
    def description(self):
        pass




"""
Конкретные Стратегии реализуют алгоритм, следуя базовому интерфейсу Стратегии.
Этот интерфейс делает их взаимозаменяемыми в Контексте.
"""


class ConcreteStrategySort(SortedStrategyBase):
    description = "Прямая сортировка"
    def do_algorithm(self, data: List) -> List:
        return sorted(data)


class ConcreteStrategySortDesc(SortedStrategyBase):
    description = "Обратная сортировка"
    def do_algorithm(self, data: List) -> List:
        return reversed(sorted(data))


if __name__ == "__main__":
    # Клиентский код выбирает конкретную стратегию и передаёт её в контекст.
    # Клиент должен знать о различиях между стратегиями, чтобы сделать
    # правильный выбор.

    context = Context(ConcreteStrategySort())
    print("Client: Strategy is set to normal sorting.")
    context.do_some_business_logic()
    print()

    print("Client: Strategy is set to reverse sorting.")
    #context.strategy = ConcreteStrategySortDesc()
    context = Context(ConcreteStrategySortDesc())
    context.do_some_business_logic()
