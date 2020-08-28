namespace Todo

module Utils =
  let tmap f a b = (f a, f b)
  let tmap2 f1 f2 a b = (f1 a, f2 b)
