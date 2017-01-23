library(readxl)

rouge_conceptual <- read_excel("D:/Tesis2016/Jarvis/Lincoln/Rougue/rouge_v3_report.xlsx", 
                               sheet = "ConceptualRST")
rouge_no_expantion_conceptual <- read_excel("D:/Tesis2016/Jarvis/Lincoln/Rougue/rouge_v3_report.xlsx", 
                               sheet = "ConceptualBest")
#View(rouge_final_conceptual)
#View(rouge_conceptual)

first_vector <- rouge_conceptual[,2]
second_vector <- rouge_no_expantion_conceptual[,2]
wilconxon = wilcox.test(first_vector, second_vector)
print(wilconxon)


print(round(mean(first_vector),3))
print(round(mean(second_vector),3))

boxplot(first_vector, second_vector)


#qqnorm(rouge1_rst_noexp)
#qqline(rouge1_rst_noexp)






